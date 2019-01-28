using System;

namespace FactoryImage2Json
{
    class Client
    {
        public Client(Toolkit toolkit, Images images)
        {
            Toolkit = toolkit ?? throw new ArgumentNullException(nameof(toolkit));
            Images = images ?? throw new ArgumentNullException(nameof(images));
        }

        public Toolkit Toolkit { get; }
        public Images Images { get; }
        
    }
}
