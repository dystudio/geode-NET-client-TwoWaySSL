using System;
using System.IO;
using Apache.Geode.Client;

namespace Humana
{
	
	/// <summary>
	/// The class is a DEMO for TWO authentication of GemFire client connections.
	/// 
	/// set LD_LIBRARY_PATH=C:\devtools\repositories\IMDG\pivotal-gemfire-native\lib
	/// set Path=%PATH%;C:\devtools\SSL\OpenSSL;
	/// 
	/// </summary>
	public class TwoWaySSL
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			
			Console.WriteLine("Two Way SSL Demo");
			
			Cache cache = Humana.TwoWaySSL.getCache();
			
			IRegion<string,string> testRegion = Humana.TwoWaySSL.getRegion("Test",cache);

			testRegion.Put("example","example",null);
			
			
			cache.Close();
			
			Console.Write("\n\n\n\n\n\n\n\nData written to region . . . ");
			Console.ReadKey(true);
			
			Environment.Exit(1);
		}
		public static Apache.Geode.Client.Cache getCache()
		{
		
			const string PwD_PASS_PROP = "SSL-KEYSTORE-PASSWORD";
		
			String password = Environment.GetEnvironmentVariable(PwD_PASS_PROP);
			
			if(password == null)
			{
				Console.WriteLine("ERROR:"+PwD_PASS_PROP+" environment variable required");
				Environment.Exit(-1);

			}
			
			// Environment.GetEnvironmentVariable(PwD_PASS_PROP);
			 string locatorHost = Environment.GetEnvironmentVariable("LOCATOR_HOST");
			
			if(locatorHost == null)
			{
				Console.WriteLine("ERROR:LOCATOR_HOST environment variable required");
				Environment.Exit(-1);

			}

			Apache.Geode.Client.Properties<string,string>  cacheProps = new Apache.Geode.Client.Properties<string,string>();
			
			cacheProps.Insert("log-level", "error");
			cacheProps.Insert("ssl-enabled", "true");
			
			cacheProps.Insert("ssl-truststore",  @"Z:\Projects\LifeSciences\Humana\dev\DigitalIT\NET\TwoWaySSL\cert\ca.cert.pem");
			cacheProps.Insert("ssl-keystore",  @"Z:\Projects\LifeSciences\Humana\dev\DigitalIT\NET\TwoWaySSL\cert\client.pem");
			cacheProps.Insert("ssl-keystore-password", "secretpassword");
			
			if(!File.Exists(cacheProps.Find("ssl-keystore")))
			{
				Console.WriteLine("ssl-keystore  does not exists");
				Environment.Exit(-1);
				
			}
			
			if(!File.Exists(cacheProps.Find("ssl-truststore")))
			{
				Console.WriteLine("ssl-truststore  does not exists");
				Environment.Exit(-1);
				
			}
				
			Apache.Geode.Client.CacheFactory factory =  Apache.Geode.Client.CacheFactory.CreateCacheFactory(cacheProps);
			factory.AddLocator(locatorHost,10000);
			Apache.Geode.Client.Cache cache = factory.Create();
			
			return cache;
		}//-------------------------------------------------------

		 public static Apache.Geode.Client.IRegion<string, string> getRegion(string name, Apache.Geode.Client.Cache cache)
		 {
		      if(string.IsNullOrEmpty(name) || cache == null){
		        throw new ArgumentNullException();
		      }
		
				Apache.Geode.Client.RegionFactory regionFactory = cache.CreateRegionFactory(Apache.Geode.Client.RegionShortcut.PROXY);
				Apache.Geode.Client.IRegion<string, string> region = regionFactory.Create<string, string>(name);
		
				return region;
		 }//------------------------------------------------------------
	}
}