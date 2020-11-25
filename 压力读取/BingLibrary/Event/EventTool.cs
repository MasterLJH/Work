using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BingLibrary.hjb.events
{
    internal class EventAggregatorRepository
    {
        public EventAggregatorRepository()
        {
            eventAggregator = new EventAggregator();
        }

        public IEventAggregator eventAggregator;
        public static EventAggregatorRepository eventRepository = null;

        //单例，保持内存唯一实例
        public static EventAggregatorRepository GetInstance()
        {
            if (eventRepository == null)
            {
                eventRepository = new EventAggregatorRepository();
            }
            return eventRepository;
        }
    }

    internal class PublishEvent : PubSubEvent { }

    public static class EventTool
    {
        private static Dictionary<string, Action> allEvents = new Dictionary<string, Action>();

        public static void SubscribeEvent(Action SubscribeEvent, string key)
        {
            if (allEvents.ContainsKey(key))
            { 
                Debug.WriteLine("■■■重复订阅：" + key + "。已采用默认值。");
            }
            else
                allEvents.Add(key, SubscribeEvent);
        }

        public static void PublishEvent(string key)
        {
            if (!allEvents.ContainsKey(key))
            {
                Debug.WriteLine("■■■未找到订阅：" + key + "。");
            }
            else
            { 
            EventAggregatorRepository.GetInstance().eventAggregator
                .GetEvent<PublishEvent>()
                .Subscribe(allEvents[key], ThreadOption.UIThread, true);

            EventAggregatorRepository.GetInstance().eventAggregator
             .GetEvent<PublishEvent>()
             .Publish();

            EventAggregatorRepository.GetInstance().eventAggregator
                .GetEvent<PublishEvent>().Unsubscribe(allEvents[key]);
            }
        }
    }

    //public class TestA
    //{
    //    public TestA()
    //    {
    //        SetSubscribe();
    //    }

    //    public void SetSubscribe()
    //    {
    //        EventAggregatorRepository
    //            .GetInstance()
    //            .eventAggregator
    //            .GetEvent<GetInputMessages>()
    //            .Subscribe(ReceiveMessage, ThreadOption.UIThread, true);
    //    }

    //    public void ReceiveMessage(string messageData)
    //    {
    //        //dosomething
    //    }
    //}

    //public class TestB
    //{
    //    public static void Print(string Meesage)
    //    {
    //        EventAggregatorRepository
    //         .GetInstance()
    //         .eventAggregator
    //         .GetEvent<GetInputMessages>()
    //         .Publish(Meesage);
    //    }
    //}
}