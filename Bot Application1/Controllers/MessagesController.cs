using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Bot_Application1
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 
        MongoClient client;
        public MessagesController()
        {
            int i = -1;
            //取得本地端MongoDB連線
            string dbName = "food";
            MongoClientSettings settings = new MongoClientSettings();
            client = new MongoClient(settings);
            var db = client.GetDatabase(dbName);
            var coll = db.GetCollection<food>("myFavorite");
            string foodname = "";
            var result = coll
             .Find(b => b.name == "i")
             .ToListAsync()
             .Result;
            foreach (var attr in result)
            {
                foodname = attr.name;
            }
            if (foodname == "")
            {
                food index = new food();
                index.name = "i";
                index.index = 0;
                coll.InsertOne(index);
            }
            //updateindex(0);
            /*
                      
             */
        }

        public int getindex()
        {
            int index = 0;
            string dbName = "food";
            var db = client.GetDatabase(dbName);
            var coll = db.GetCollection<food>("myFavorite");
            var result = coll
             .Find(b => b.name == "i")
             .ToListAsync()
             .Result;
            foreach (var attr in result)
            {
                index = attr.index;
            }
            return index;
        }



        public void updateindex(int index1)
        {
            string dbName = "food";
            var db = client.GetDatabase(dbName);

            List<food> foodlist = new List<food>();
            var coll = db.GetCollection<food>("myFavorite");

            var result = coll.FindOneAndUpdateAsync(
                                Builders<food>.Filter.Eq("name", "i"),
                                Builders<food>.Update.Set("index", index1)
                                );
        }






        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {

                Activity reply = activity.CreateReply();//activity 為從simulator收到的訊息
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));//connector為bot和simulator中間的橋樑
                int i = 0;



                if (activity.Text.StartsWith("我要看"))
                {

                    //reply.Text = getInfo();
                    reply.Text = "不給看";
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                else
                {
                    i = getindex();
                    if (i == 0)
                    {
                        reply.Text = "Hello 你好啊!";
                        updateindex(1);

                        await connector.Conversations.ReplyToActivityAsync(reply);//傳送訊息
                    }
                    else if (i == 1)
                    {
                        reply.Text = "你最近過的怎麼樣壓";
                        updateindex(2);

                        await connector.Conversations.ReplyToActivityAsync(reply);//傳送訊息
                    }
                    else if (i == 2)
                    {
                        reply.Text = "有機會再一起出來玩";
                        updateindex(3);

                        await connector.Conversations.ReplyToActivityAsync(reply);//傳送訊息
                    }
                    else if (i == 3)
                    {
                        updateindex(0);

                        reply.Text = "下次見啦!";

                        await connector.Conversations.ReplyToActivityAsync(reply);//傳送訊息
                    }
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
    public class food
    {
        public ObjectId _id { get; set; }
        public string price { get; set; }//食物價錢
        public string name { get; set; }//食物名子
        public int place { get; set; }//食物地方
        public string rank { get; set; }//排名
        public int index { get; set; }
    }
}