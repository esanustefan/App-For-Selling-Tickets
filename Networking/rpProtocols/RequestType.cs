using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking.rpProtocols
{
    public enum RequestType
    {
        GET_USERS,
        FIND_USER,
        CHECK_SEATS,
        FIND_MECI,
        BUY_TICKET,
        MECIURI,
        ADD_TICKET,
        GET_MECIURI,
        GET_TICKETS,
        LOGIN,
        LOGOUT,
        UPDATE_MECI
    }
}