using System;
using System.IO;
using Apache.Geode.Client;

namespace Humana
{
	
	/// <summary>
	/// The class is a DEMO for TWO authentication of GemFire client connections.
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
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
			
			cache.Close();
			Environment.Exit(1);
		}
		
		
		//private const string truststore = @"Z:\Projects\LifeSciences\Humana\dev\DigitalIT\NET\TwoWaySSL\cert\gemfire1.keystore";

		public static Apache.Geode.Client.Cache getCache()
		{
		
		

			Apache.Geode.Client.Properties<string,string>  cacheProps = new Apache.Geode.Client.Properties<string,string>();
			
			cacheProps.Insert("log-level", "fine");
			cacheProps.Insert("ssl-enabled", "true");
			//cacheProps.Insert("security-client-kspath",@"Z:\Projects\LifeSciences\Humana\dev\DigitalIT\NET\TwoWaySSL\cert\gemfire1.pem");
			//cacheProps.Insert("ssl-keystore",@"Z:\Projects\LifeSciences\Humana\dev\DigitalIT\NET\TwoWaySSL\cert\gemfire1.keystore");
			//cacheProps.Insert("ssl-truststore", @"Z:\Projects\LifeSciences\Humana\dev\DigitalIT\NET\TwoWaySSL\cert\certificatetruststore" );
			//cacheProps.Insert("security-client-kspasswd","humana123");
			//cacheProps.Insert("security-alias","gemfire1");
			
			cacheProps.Insert("ssl-truststore",  @"Z:\Projects\LifeSciences\Humana\dev\DigitalIT\NET\TwoWaySSL\cert\ca.cert.pem");
			cacheProps.Insert("ssl-keystore",  @"Z:\Projects\LifeSciences\Humana\dev\DigitalIT\NET\TwoWaySSL\cert\client.pem");
			cacheProps.Insert("ssl-keystore-password", "secretpassword");
			
			cacheProps.Insert("security-keystorepass","humana123");

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
			factory.AddLocator("ec2-34-232-109-123.compute-1.amazonaws.com",10000);
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