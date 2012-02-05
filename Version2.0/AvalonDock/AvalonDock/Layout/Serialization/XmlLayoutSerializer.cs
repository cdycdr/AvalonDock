using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AvalonDock.Layout.Serialization
{
    public class XmlLayoutSerializer : LayoutSerializer
    {
        public XmlLayoutSerializer(DockingManager manager)
            : base(manager)
        { 
        
        }

        public void Serialize(System.Xml.XmlWriter writer)
        {
            var serializer = new XmlSerializer(typeof(LayoutRoot));
            serializer.Serialize(writer, Manager.Layout);
        }
        public void Serialize(System.IO.TextWriter writer)
        {
            var serializer = new XmlSerializer(typeof(LayoutRoot));
            serializer.Serialize(writer, Manager.Layout);
        }
        public void Serialize(System.IO.Stream stream)
        {
            var serializer = new XmlSerializer(typeof(LayoutRoot));
            serializer.Serialize(stream, Manager.Layout);
        }

        public void Deserialize(System.IO.Stream stream)
        {
            var serializer = new XmlSerializer(typeof(LayoutRoot));
            var layout = serializer.Deserialize(stream) as LayoutRoot;
            FixupLayout(layout);
            Manager.Layout = layout;
        }
        public void Deserialize(System.IO.TextReader reader)
        {
            var serializer = new XmlSerializer(typeof(LayoutRoot));
            var layout = serializer.Deserialize(reader) as LayoutRoot;
            FixupLayout(layout);
            Manager.Layout = layout;
        }
        public void Deserialize(System.Xml.XmlReader reader)
        {
            var serializer = new XmlSerializer(typeof(LayoutRoot));
            var layout = serializer.Deserialize(reader) as LayoutRoot;
            FixupLayout(layout);
            Manager.Layout = layout;
        }
    }
}
