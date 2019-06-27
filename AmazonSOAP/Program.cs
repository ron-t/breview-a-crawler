using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.PAAPI.WCF;
using System.ServiceModel;
using Amazon.PAAPI.AWS;

namespace Amazon.PAAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instantiate Amazon ProductAdvertisingAPI client
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = int.MaxValue;
            AWSECommerceServicePortTypeClient amazonClient = new AWSECommerceServicePortTypeClient(
                binding,
                new EndpointAddress("https://webservices.amazon.com/onca/soap?Service=AWSECommerceService"));

            //Adding this uses the code in Amazon.PAAPI.WCF to sign requests automagically
            amazonClient.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior());

            // prepare an ItemSearch request
            ItemSearchRequest request = new ItemSearchRequest();
            request.SearchIndex = "DVD";
            request.Title = "Matrix";
            request.ResponseGroup = new string[] { "Small" };

            ItemSearch itemSearch = new ItemSearch();
            itemSearch.Request = new ItemSearchRequest[] { request };
            itemSearch.AssociateTag = "notag"; //this is a required param, so I just use a dummy value which seems to work

            // send the ItemSearch request
            ItemSearchResponse response = amazonClient.ItemSearch(itemSearch);

            // write out the results from the ItemSearch request
            foreach (var item in response.Items[0].Item)
            {
                Console.WriteLine(item.ItemAttributes.Title);
            }
            Console.WriteLine("\n\n<<< done...enter any key to continue >>>");
            Console.ReadLine();
        }
    }
}
