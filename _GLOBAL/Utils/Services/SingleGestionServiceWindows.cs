using _Global.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.Utils.Services {
    public class SingleGestionServiceWindows {

        public List<ServiceInfos> listServiceInfos;

        #region Constructeur Singleton
        protected static SingleGestionServiceWindows instance;

        protected SingleGestionServiceWindows() {
            listServiceInfos = new List<ServiceInfos>();
        }

        public static SingleGestionServiceWindows Instance() {
            if (instance == null)
                instance = new SingleGestionServiceWindows();
            return instance;
        }
        #endregion

        private void ReadSvcs() {
            //Vecteur contenant la liste des services 
            ServiceController[] ListSvcs;
            //Lire la liste des services. 
            ListSvcs = ServiceController.GetServices();

            foreach (ServiceController SingleSvc in ListSvcs) {
                listServiceInfos.Add(GetInfos(SingleSvc));
            }
        }

        public ServiceInfos GetInfosServiceName(string ServiceName) {
            var ListSvcs = ServiceController.GetServices();
            var serv = ListSvcs.Where(c => c.ServiceName.ToUpper() == ServiceName.ToUpper()).FirstOrDefault();
            if (serv != null) {
                return GetInfos(serv);
            }
            return null;
        }

        private ServiceInfos GetInfos(ServiceController serviceController) {
            var serv = new ServiceInfos() {
                DisplayName = serviceController.DisplayName,
                ServiceName = serviceController.ServiceName,
                Status = serviceController.Status.ToString(),
                ServiceType = serviceController.ServiceType.ToString(),
                NotRunning = serviceController.Status != ServiceControllerStatus.Running
            };
            System.Text.StringBuilder dependedServices = new System.Text.StringBuilder(5000, 10000);
            foreach (var subserv in serviceController.DependentServices) {
                dependedServices.Append(subserv.DisplayName);
                dependedServices.Append("; ");
            }
            serv.SubItems = dependedServices.ToString();
            return serv;
        }


        public bool StopService(string ServiceName) {
            var ListSvcs = ServiceController.GetServices();
            var serv = ListSvcs.Where(c => c.ServiceName.ToUpper() == ServiceName.ToUpper()).FirstOrDefault();
            if (serv != null) {
                try {
                    serv.Stop();  // il faut être en administrateur pour faire ça !!
                    return true;
                }
                catch (Exception ex) {
                    GlobalLog.Instance().AjouteLog("SingleGestionServiceWindows", "@StopService : " + ex.Message);
                }
            }
            return false;
        }

        public bool StartService(string ServiceName) {
            var ListSvcs = ServiceController.GetServices();
            var serv = ListSvcs.Where(c => c.ServiceName.ToUpper() == ServiceName.ToUpper()).FirstOrDefault();
            if (serv != null) {
                try {
                    serv.Start();    // il faut être en administrateur pour faire ça !!
                    return true;
                }
                catch (Exception ex) {
                    GlobalLog.Instance().AjouteLog("SingleGestionServiceWindows", "@StartService : " + ex.Message);
                }
            }
            return false;
        }

        public bool PauseService(string ServiceName) {
            var ListSvcs = ServiceController.GetServices();
            var serv = ListSvcs.Where(c => c.ServiceName.ToUpper() == ServiceName.ToUpper()).FirstOrDefault();
            if (serv != null) {
                try {
                    if (serv.CanPauseAndContinue) {
                        serv.Pause();
                        return true;
                    }
                    else {
                        //MessageBox.Show("Le service ne supporte pas cette opération", "Erreur");
                        return false;
                    }
                }
                catch (Exception ex) {
                    GlobalLog.Instance().AjouteLog("SingleGestionServiceWindows", "@PauseService : " + ex.Message);
                }
            }
            return false;
        }

    }


    public class ServiceInfos {
        public string DisplayName;
        public string ServiceName;
        public string Status;
        public bool NotRunning;
        public string ServiceType;
        public string SubItems;
    }
}
