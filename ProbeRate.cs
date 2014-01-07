using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinForm_StressTest
{
    enum ACTIONTYPE { LOGAPPEND, DOWNLOAD, UPLOAD };    // definition for actiontype

    class ProbeRate
    {
        private double token_unit;
        private int threads_num;
        private int profile_time_interval_in_ms;
        private bool isUnlimitedMode;
        private double limitrate;
        private RateProfile profile;
        private TokenController controller;
        private Action action;

        public ProbeRate(double token_unit, int threads_num,
                        int profile_time_interval_in_ms, bool isUnlimitedMode, double limitrate = 0)
        {
            if (token_unit <= 0 || threads_num <= 0 || profile_time_interval_in_ms <= 0 ||
                    (!isUnlimitedMode && limitrate <= 0))
                throw new ArgumentOutOfRangeException();

            this.token_unit = token_unit;
            this.threads_num = threads_num;
            this.profile_time_interval_in_ms = profile_time_interval_in_ms;
            this.isUnlimitedMode = isUnlimitedMode;
            this.limitrate = isUnlimitedMode ? 0 : limitrate;
        }

        public void ProbeRun(Update update_data, MessageNotify msg_notify, 
                ACTIONTYPE actiontype, string Deststream, string SourceStream = "")
        {
            if (Deststream == String.Empty ||
                    (actiontype > ACTIONTYPE.LOGAPPEND && SourceStream == String.Empty))
                throw new ArgumentOutOfRangeException();

            profile = new RateProfile(profile_time_interval_in_ms, update_data);
            if (isUnlimitedMode)
                controller = new TokenController(token_unit);
            else
                controller = new TokenController(limitrate, token_unit);

            switch (actiontype)
            {
                case ACTIONTYPE.LOGAPPEND:
                    action = new LogAppendAction(threads_num, ref controller, ref profile, Deststream, SourceStream, 
                                            msg_notify);
                    break;
                case ACTIONTYPE.DOWNLOAD:
                    action = new DownloadAction(threads_num, ref controller, ref profile, Deststream, SourceStream, 
                                            msg_notify);
                    break;
                case ACTIONTYPE.UPLOAD:
                    action = new UploadAction(threads_num, ref controller, ref profile, Deststream, SourceStream, 
                                            msg_notify);
                    break;
            }

            action.ActionStart();
        }

        public void ProbeStop()
        {
            if (action != null)
                action.ActionStop();
        }

        public double GetAverageRate()
        {
            if (profile != null)
                return profile.GetAverageRate();
            else
                return 0;
        }
    }
}
