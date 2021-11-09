using System;

namespace Server
{
    public class TitleAlreadyExistsException : Exception
    {
        public override string Message => "Ya existe un juego con ese título";
    }
}
