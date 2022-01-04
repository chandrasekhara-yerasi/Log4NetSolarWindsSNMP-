using System;
using System.Reflection;
using SolarWinds.Net.SNMP;
namespace Log4NetSolarWindsSNMP
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
            currentDomain.AssemblyLoad += new AssemblyLoadEventHandler(MyLoadEventHandler);

            testResolveForStrongNameAssembliesWithDifferentPublicTokens();

            Console.WriteLine("Solarwinds SNMP start");

            SNMPPriv snmpPriv = SNMPPriv.None;
                SNMPAuth snmpAuth = SNMPAuth.None;
                SNMPRequest request = new SNMPRequest();
                request.SessionHandle.AuthType = snmpAuth;
                request.SessionHandle.PrivacyType = snmpPriv;
/*           } catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return;
            }*/
            Console.WriteLine("Solarwinds SNMP end");
        }
        private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("Resolving..." + args.Name);
            Assembly dll = null;
            if (args.Name.Contains("log4net"))
            {
                string log4netLocation = AppDomain.CurrentDomain.BaseDirectory + "log4net.dll";
                Console.WriteLine(log4netLocation);
                dll = Assembly.LoadFrom(log4netLocation);
                Console.WriteLine("New log4net assembly: " + dll.FullName);
            }
            return dll;
        }
        private static void  MyLoadEventHandler(object sender, AssemblyLoadEventArgs args)
        {
            AssemblyName loadedAssembly = args.LoadedAssembly.GetName();
            Console.WriteLine("MyLoadEventHandler: " + loadedAssembly.FullName);
            if(loadedAssembly.Name.Contains("log4net"))
            {

            }
        }

        private static void testResolveForStrongNameAssembliesWithDifferentPublicTokens()
        {
            String type = "log4net.Core.LogException";
            String assemblyWithDifferentPublicToken = "log4net, Version=1.2.10.0, Culture=neutral, PublicKeyToken=1b44e1d426115821";
            String assemblyWithSamePublicToken = "log4net, Version=1.2.10.0, Culture=neutral, PublicKeyToken=669e0ddf0bb1aa2a";

            Console.WriteLine("Solarwinds strong name assembly test start");

            LoadStrongNameAssemblyAndCreateTypeInstance(assemblyWithDifferentPublicToken, type); // PublicKeyToken different from log4net 2.x dll version and hence cannot be resolved
            LoadStrongNameAssemblyAndCreateTypeInstance(assemblyWithSamePublicToken, type); // PublicKeyToken same as log4net 2.x dll version and hence can be resolved

            Console.WriteLine("Solarwinds strong assembly test end");

        }
        private static void LoadStrongNameAssemblyAndCreateTypeInstance(string assemblyName, String type)
        {
            AppDomain ad = AppDomain.CurrentDomain;

            try
            {
                object obj = ad.CreateInstanceAndUnwrap(assemblyName, type);
                Console.WriteLine("Resolved {0} from {1}", type, assemblyName);
                            
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during Load " + ex.Message);
            }
        }

    }
}