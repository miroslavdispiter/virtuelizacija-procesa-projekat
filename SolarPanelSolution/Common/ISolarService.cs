using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface ISolarService
    {
        [OperationContract]
        bool StartSession(PvMeta meta);

        [OperationContract]
        bool PushSample(PvSample sample);

        [OperationContract]
        bool EndSession();
    }
}
