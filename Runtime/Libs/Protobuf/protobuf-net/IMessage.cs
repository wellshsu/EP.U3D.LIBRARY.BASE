namespace ProtoBuf
{
    public interface IMessage
    {
        void Encode(Google.Protobuf.CodedOutputStream writer);
        void Decode(Google.Protobuf.CodedInputStream reader);
    }
}