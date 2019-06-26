using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace _GLOBAL.Utils.Performance {

    public class SinglePerformanceLogger {

        //Stopwatch sw = Stopwatch.StartNew();
        //sw.Stop();
        private Object thisLock = new Object();  // ajout d'une section critique
        List<LogGroupCallPerformanceItem> listLogGroupCallPerformanceItem;
        long RecordMaxTime = 500;   


        #region Constructeur Singleton
        protected static SinglePerformanceLogger instance;

        protected SinglePerformanceLogger() {
            listLogGroupCallPerformanceItem = new List<LogGroupCallPerformanceItem>();
        }

        public static SinglePerformanceLogger Instance() {
            if (instance == null)
                instance = new SinglePerformanceLogger();
            return instance;
        }
        #endregion

        public ContainerWatch StartStopWatch(HttpRequest httpRequest) {
            ContainerWatch cw = new ContainerWatch(httpRequest);
            return cw;
        }

        public long StopStopWatch(ContainerWatch cw) {
            lock (thisLock) {
                var result = cw.StopStopWatch();
                long timeFonction = result.timeFonction;
                //if (timeFonction >= RecordMaxTime) {
                //}
                var exist = listLogGroupCallPerformanceItem.Where(c => c.sender == result.sender).FirstOrDefault();
                if (exist == null) {
                    exist = new LogGroupCallPerformanceItem() { sender = result.sender, CptCall = 0, lastCall = DateTime.MaxValue };
                    listLogGroupCallPerformanceItem.Add(exist);
                }
                exist.CptCall++;
                DateTime now = DateTime.Now;
                if (exist.lastCall < now) {
                    var delay = now - exist.lastCall;
                    exist.AverageDelaiCall = (long)delay.TotalMilliseconds;
                }
                exist.lastCall = now;
                if (result.timeFonction >= exist.MaxTimeFonction) {
                    exist.MaxTimeFonction = result.timeFonction;
                    exist.lastMaxLogCallPerformanceItem = result;
                    exist.WarningTimeMax = exist.MaxTimeFonction > RecordMaxTime;
                }
                exist.AverageTimeFonction += result.timeFonction;
                exist.AverageTimeFonction = exist.AverageTimeFonction / exist.CptCall;
                //exist.AverageDelaiCall
                //exist.AverageTimeFonction
                return timeFonction;
            }
        }


        public ResultPerformanceLogger GetSetPerformanceLogger(ParamPerformanceLogger paramPerformanceLogger) {
            lock (thisLock) {
                ResultPerformanceLogger resultPerformanceLogger = new ResultPerformanceLogger() { listLogCallPerformanceItem = new List<LogGroupCallPerformanceItem>(), Message = "PB param" };
                if (paramPerformanceLogger != null && paramPerformanceLogger.Action != null) {
                    if (paramPerformanceLogger.Action == "Reset") {
                        listLogGroupCallPerformanceItem.Clear();
                    }
                    if (paramPerformanceLogger.Action == "SetMax") {
                        long max = paramPerformanceLogger.Param;
                        if (max < 10) {
                            max = 10;
                        }
                        RecordMaxTime = max;
                    }
                    resultPerformanceLogger.Message = "Action : " + paramPerformanceLogger.Action + "  MaxTimeRecord = " + RecordMaxTime.ToString();
                    resultPerformanceLogger.listLogCallPerformanceItem.AddRange(listLogGroupCallPerformanceItem);
                }

                return resultPerformanceLogger;
            }
        }

    }

    public class ResultPerformanceLogger {
        public long ExecutionTimeMs;
        public string Message;
        public List<LogGroupCallPerformanceItem> listLogCallPerformanceItem;
    }

    public class ParamPerformanceLogger {
        public string Action;
        public long Param;
    }


    public class ContainerWatch {
        Stopwatch sw;
        List<MarkTime> listMarkTime;
        DateTime dtCall;
        public long timeFonction;
        string sender;

        public ContainerWatch() {
            listMarkTime = new List<MarkTime>();
            sender = "Anonyme";
            sw = Stopwatch.StartNew();
            dtCall = DateTime.Now;
        }

        public ContainerWatch(HttpRequest httpRequest) {
            listMarkTime = new List<MarkTime>();
            sender = httpRequest.UserHostAddress + " -> " + httpRequest.HttpMethod + " : " + httpRequest.RawUrl;
            sw = Stopwatch.StartNew();
            dtCall = DateTime.Now;
        }

        public void MarkTime(string sMarque) {
            listMarkTime.Add(new MarkTime() { Mark = sMarque, Delay = sw.ElapsedMilliseconds });
        }

        public LogCallPerformanceItem StopStopWatch() {
            sw.Stop();
            timeFonction = sw.ElapsedMilliseconds;
            LogCallPerformanceItem logCallPerformanceItem = new LogCallPerformanceItem() { sender = sender, timeFonction = timeFonction, dt = dtCall , listMarkTime = new List<MarkTime>()};
            logCallPerformanceItem.listMarkTime.AddRange(listMarkTime);
            return logCallPerformanceItem;
        }
    }

    public class MarkTime {
        public string Mark;
        public long Delay;
    }


    public class LogCallPerformanceItem {
        public string sender;
        public DateTime dt;
        public long timeFonction;
        public List<MarkTime> listMarkTime;
    }


    public class LogGroupCallPerformanceItem {
        public string sender;
        public LogCallPerformanceItem lastMaxLogCallPerformanceItem;
        public long MaxTimeFonction;
        public long AverageTimeFonction;
        public bool WarningTimeMax;
        public int CptCall;
        public DateTime lastCall;
        public long AverageDelaiCall;
    }
}
