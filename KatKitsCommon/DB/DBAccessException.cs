namespace KatKits.DB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DBAccessRollbackException:Exception
    {
        public readonly Exception OriginalException;
        public readonly Exception RollbackException;
        public DBAccessRollbackException(Exception OriginalException,Exception RollbackException) : base("Exception catched in TRAN rollback")
        {
            this.OriginalException = OriginalException;
            this.RollbackException= RollbackException;
        }
    }
}
