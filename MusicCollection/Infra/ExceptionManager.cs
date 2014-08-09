using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Mail;

using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.Implementation;

namespace MusicCollection.Fundation
{
    public class ExceptionManager
    {
        private Exception _Ex;
        private ApplicationTraceListener _ATL;
        private DateTime _DT;
        private IEmailFactory _IEmailFactory;

        public ExceptionManager(IMusicSession ims, object Exception)
        {
            _Ex = Exception as Exception;
            _ATL = (ims as MusicSessionImpl).TraceListener;
            _DT = DateTime.Now;
            _IEmailFactory = ims.GetEmailFactory();
        }

        private void FromTraces(StringBuilder sb)
        {
            sb.Append(string.Format("Trace Message:"));
            sb.AppendLine();

            foreach(Tuple<DateTime, string> tu in _ATL.Events)
            {
                sb.Append(string.Format("{0}: {1}", tu.Item1,tu.Item2));
                sb.AppendLine();
            }
      
        }

        static private void FromException(Exception e, StringBuilder sb)
        {
            sb.Append(string.Format("Exception Message {0} ",e.Message));
            sb.AppendLine();

            sb.Append(string.Format("Exception Type {0} ", e.GetType().ToString()));
            sb.AppendLine();

            sb.Append(string.Format("Exception Source {0} ", e.Source));
            sb.AppendLine();

            sb.Append(string.Format("Exception Stack {0} ", e.StackTrace));
            sb.AppendLine();

            Exception inner = e.InnerException;

            if (inner != null)
            { 
                sb.Append(string.Format("Inner Exception -----------------------"));
                sb.AppendLine();
                FromException(inner, sb);
                sb.Append("-----------------------------------------");
                sb.AppendLine();
            }

        }

        private string FromException(Exception e)
        {
            StringBuilder ab = new StringBuilder();
            ab.Append("Unhandled Exception detected");
            ab.Append(string.Format("Music Collection V{0}",VersionInfo.GetVersionInfo().ToString()));
            ab.AppendLine();
            ab.Append(string.Format("Problem detected at {0}",this._DT));
            ab.AppendLine();
            this.FromTraces(ab);
            FromException(e, ab);
            return ab.ToString();     
        }

        public void Deal(bool sendemail)
        {
            if (_Ex == null)
                return;

            if (sendemail == false)
                return; 

            IEmail aem = _IEmailFactory.GetEmail();
            aem.Title = "UnHandled Error detected in a Music Collection";
            aem.Message = FromException(_Ex);

            aem.Send();
        }
    }
}
