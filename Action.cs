using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WinForm_StressTest
{
    class Action
    {
        private int threads_num;
        private Thread[] Thread_sets;
        protected volatile bool[] Run_flags;
        protected TokenController Controller;
        protected RateProfile Profile;
        protected string testdata;
        protected int testdata_size;
        protected MessageNotify msg_notify;
        protected string deststream;
        protected string sourcestream;

        public Action(int threads, ref TokenController Controller, ref RateProfile Profile, 
                      string DestStream, string SourceStream, MessageNotify msg_notify)
        {
            if (threads <= 0 || Controller == null || Profile == null)
                throw new ArgumentOutOfRangeException();
            threads_num = threads;
            Thread_sets = new Thread[threads_num];
            Run_flags = new bool[threads_num];

            for (int i = 0; i < threads_num; i++)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(ActionOn));
                Thread_sets[i] = thread;
                Run_flags[i] = true;
            }

            this.Controller = Controller;
            this.Profile = Profile;
            deststream = DestStream;
            sourcestream = SourceStream;
            testdata_size = (int)Controller.GetTokenUnit();
            testdata = new string('a', testdata_size * 1024);
            if (msg_notify != null)
                this.msg_notify = msg_notify;
            else
                this.msg_notify = default_notify;
        }

        ~Action()
        {
        }

        public void ActionStart()
        {
            Controller.ControllerStart();
            Profile.ProfileStart();
            for (int i = 0; i < threads_num; i++)
                Thread_sets[i].Start(i);
        }

        public void ActionStop()
        {
            for (int i = 0; i < threads_num; i++)
                Run_flags[i] = false;

            for (int i = 0; i < threads_num; i++)
                Thread_sets[i].Join();

            Controller.ControllerStop();
            Profile.ProfileStop();
        }

        public void ActionThreadExit(int index)
        {
            
        }

        public virtual void ActionOn(object index) // things need to be done
        {
        }

        public void default_notify(string str)
        {
            Console.WriteLine(str);
        }
    }
}
