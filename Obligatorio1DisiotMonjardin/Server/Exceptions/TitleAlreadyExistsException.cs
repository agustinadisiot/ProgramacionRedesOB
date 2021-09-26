using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class TitleAlreadyExistsException : Exception
    {
        public override string Message => "Ya existe un juego con ese título";
    }
}
