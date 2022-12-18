using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLAccess.Services
{
    public interface ISpeechLogger
    {
        void SpeakAsync(string text, bool sayDate, bool sayFast);
    }
}
