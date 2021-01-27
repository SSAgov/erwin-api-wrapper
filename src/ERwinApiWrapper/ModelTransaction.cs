using SCAPI;
using PrimitiveExtensions;
using System;
using System.Data; 

namespace ERwinApiWrapper
{
    public class ModelTransaction
    {
        public ModelTransaction(Model m)
        {
            Model = m;
            TransactionID = m.ModelingSession.BeginTransaction();
        }

        public object TransactionID { get; private set; 
    }

        public Model Model { get; private set; }


        public void Commit()
        {
            Model.ModelingSession.CommitTransaction(TransactionID);
        }
        
        public void Rollback()
        {
            Model.ModelingSession.RollbackTransaction(TransactionID);
        }
    }
}