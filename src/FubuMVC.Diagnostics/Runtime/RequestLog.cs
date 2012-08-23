using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime.Logging;
using FubuMVC.SlickGrid;

namespace FubuMVC.Diagnostics.Runtime
{
    public class RequestLog
    {
        private readonly IList<RequestStep> _steps = new List<RequestStep>();

        public RequestLog()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public Guid BehaviorId { get; set; }

        public double ExecutionTime { get; set; }


        public string Url { get; set; }
        public string ReportUrl { get; set; }
        public string HttpMethod { get; set; }
        public DateTime Time { get; set; }

        public void AddLog(double requestTimeInMilliseconds, object log)
        {
            _steps.Add(new RequestStep(requestTimeInMilliseconds, log));
        }

        public HttpStatus HttpStatus
        {
            get
            {
                HttpStatusReport report = null;

                var log = _steps.Where(x => x.Log is HttpStatusReport).LastOrDefault();
                if (log != null)
                {
                    report = log.Log.As<HttpStatusReport>();
                }
                
                return new HttpStatus(report, Failed);
            }
        }

        public string LocalTime
        {
            get
            {
                return Time.ToLocalTime().ToShortTimeString();
            }
        }

        public string ContentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool Failed { get; set; }

        public IEnumerable<RequestStep> AllSteps()
        {
            return _steps;
        }

        public bool Equals(RequestLog other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id.Equals(Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RequestLog)) return false;
            return Equals((RequestLog) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Id: {0}", Id);
        }
    }

    public class HttpStatus : IMakeMyOwnJsonValue
    {
        private readonly HttpStatusCode _status;
        private readonly string _description;

        public HttpStatus(HttpStatusReport report, bool failed)
        {
            if(report != null)
            {
                _status = report.Status;
                _description = report.Description;
            }
            else
            {
                _status = failed ? HttpStatusCode.InternalServerError : HttpStatusCode.OK;
            }
           
            if (_description.IsEmpty())
            {
                _description = _status.ToString().SplitPascalCase();
            }
        }

        public HttpStatusCode Status
        {
            get { return _status; }
        }

        public string Description
        {
            get { return _description; }
        }

        public object ToJsonValue()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("code", _status.As<int>().ToString());
            dict.Add("description", _description);

            return dict;
        }
    }

    public class RequestStep
    {
        public RequestStep(double requestTimeInMilliseconds, object log)
        {
            RequestTimeInMilliseconds = requestTimeInMilliseconds;
            Log = log;
        }

        public double RequestTimeInMilliseconds { get; private set; }
        public object Log { get; private set; }

        public bool Equals(RequestStep other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.RequestTimeInMilliseconds.Equals(RequestTimeInMilliseconds) && Equals(other.Log, Log);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RequestStep)) return false;
            return Equals((RequestStep) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (RequestTimeInMilliseconds.GetHashCode()*397) ^ (Log != null ? Log.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("RequestTimeInMilliseconds: {0}, Log: {1}", RequestTimeInMilliseconds, Log);
        }
    }
}