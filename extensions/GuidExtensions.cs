using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class GuidExtensions {

    public static bool IsNullOrEmpty(this Guid inst) {
        if (inst != null && inst != new Guid()) {
            return true;
        }
        return false;
    }

    public static Guid Zero() {
        return new Guid("00000000-0000-0000-0000-000000000000");
    }

    public static Guid Generate() {
        return System.Guid.NewGuid();
    }
}
