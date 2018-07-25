/*******************************************************************************
* Copyright 2009-2018 Amazon.com, Inc. or its affiliates. All Rights Reserved.
* 
* Licensed under the Apache License, Version 2.0 (the "License"). You may
* not use this file except in compliance with the License. A copy of the
* License is located at
* 
* http://aws.amazon.com/apache2.0/
* 
* or in the "license" file accompanying this file. This file is
* distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
* KIND, either express or implied. See the License for the specific
* language governing permissions and limitations under the License.
*******************************************************************************/

using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace SNSSample1
{
    class Program
    {
        static void Main(string[] args)
        {
            var sns = new AmazonSimpleNotificationServiceClient();
            string httpAddress = "http://34.217.101.161:3018/redisServer";

            try
            {
                // Create topic
                Console.WriteLine("Creating topic...");
                var topicArn = sns.CreateTopic(new CreateTopicRequest
                {
                    Name = "SampleSNSTopic"
                }).TopicArn;

                // Set display name to a friendly value
                Console.WriteLine();
                Console.WriteLine("Setting topic attributes...");
                sns.SetTopicAttributes(new SetTopicAttributesRequest
                {
                    TopicArn = topicArn,
                    AttributeName = "DisplayName",
                    AttributeValue = "Sample Notifications"
                });

                // Subscribe an endpoint - in this case, an email address
                Console.WriteLine();
                Console.WriteLine("Subscribing http address {0} to topic...", httpAddress);
                sns.Subscribe(new SubscribeRequest
                {
                    TopicArn = topicArn,
                    Protocol = "http",
                    Endpoint = httpAddress
                });

                // When using http, recipient must confirm subscription
                Console.WriteLine();
                Console.WriteLine("Please check your http endpoint and press enter when you are subscribed...");
                Console.ReadLine();

                // Publish message
                Console.WriteLine();
                _ = DespatchAsync(@"[ '967813618981',
                                'BPNYDCFEE-6592',
                                '2/2/2018 20:59:00',
                                'Ops Hold',
                                '2018-02-02T20:59:38',
                                'Hold',
                                'Hold',
                                '',
                                '',
                                '' ]", topicArn);
                _ =DespatchAsync(@"[ '967813618981',
                                'BPNYDCFEE-6592',
                                '2/2/2018 20:59:00',
                                'Ops Hold',
                                '2018-02-02T20:59:38',
                                'Hold',
                                'Hold',
                                '',
                                '',
                                '' ]", topicArn);
                // Verify http endpoint receieved
                Console.WriteLine();
                Console.WriteLine("Please check your http endpoint and press enter when you receive the message...");
                Console.ReadLine();

                // Delete topic
                Console.WriteLine();
                Console.WriteLine("Deleting topic...");
                sns.DeleteTopic(new DeleteTopicRequest
                {
                    TopicArn = topicArn
                });
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
            }

            Console.WriteLine();
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        public static async Task<PublishResponse> DespatchAsync(string message,string topicArn)
        {
            Console.WriteLine("Publishing message to topic...");
            var sns = new AmazonSimpleNotificationServiceClient();
            if (string.IsNullOrEmpty(topicArn) || string.IsNullOrEmpty(message))
            {
                return new PublishResponse(); //do not proceed
            }

            var request = new PublishRequest(topicArn, message);
            var response = await sns.PublishAsync(request);
            return response;
        }


        public static void pushMessage(string message, string topicArn)
        {
            Console.WriteLine("Publishing message to topic...");
            var sns = new AmazonSimpleNotificationServiceClient();

            sns.Publish(new PublishRequest
            {
                Subject = "Test",
                Message = message,
                TopicArn = topicArn
            });
        }
    }
}