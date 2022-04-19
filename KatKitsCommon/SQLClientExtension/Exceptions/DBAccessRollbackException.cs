namespace KatKits.SQLClientExtension.Exceptions {
  using System;
  using System.Collections.Generic;
  using System.Text;

  public class DBAccessRollbackException : Exception {
    public readonly Exception OriginalException;
    public readonly Exception RollbackException;
    public DBAccessRollbackException(Exception OriginalException, Exception RollbackException) : base("Exception catched in TRAN rollback") {
      this.OriginalException = OriginalException;
      this.RollbackException = RollbackException;
    }
  }
}
