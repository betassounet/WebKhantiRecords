using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.Log {

    public enum eSenderMessage { NC, Serialisation, SerialisationJson, SerialisationDataContract, PLC, Config, InitCircuits };
    public enum eStatutMessage { OK, Warning, InitErreur, Erreur, Exception };

    public class SingleMessageStatut {
        public delegate void NotifyMessage(MessageStatut messageStatut);
        public event NotifyMessage onNotifyMessage;

        public List<MessageStatut> listMessageStatut;

        #region constructeur singleton

        protected static SingleMessageStatut instance;

        protected SingleMessageStatut() {
            listMessageStatut = new List<MessageStatut>();
        }

        public static SingleMessageStatut Instance() {
            if (instance == null)
                instance = new SingleMessageStatut();
            return instance;
        }
        #endregion


        public void AddMessageStatut(string sMessage, eSenderMessage SenderMessage = eSenderMessage.NC, eStatutMessage StatutMessage = eStatutMessage.OK, string Loc = "NC") {
            MessageStatut messageStatut = new MessageStatut() { dt = DateTime.Now, sMessage = sMessage, SenderMessage = SenderMessage, StatutMessage = StatutMessage, Loc = Loc };
            listMessageStatut.Add(messageStatut);
            if (onNotifyMessage != null) {
                onNotifyMessage(messageStatut);
            }
        }

    }

    public class MessageStatut {
        public DateTime dt;
        public string sMessage;
        public eSenderMessage SenderMessage;
        public eStatutMessage StatutMessage;
        public string Loc;
    }
}
