using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace EZ_B {
  internal class TcpClient {

    StreamSocket _socket;

    public int ReceiveTimeout {
      get; set;
    }

    public int SendTimeout {
      get; set;
    }

    public bool IsConnected {
      get {
        return _socket != null;
      }
    }

    public TcpClient() {

      ReceiveTimeout = 5000;
      SendTimeout = 5000;
    }

    public async Task Connect(string host, int port, int timeoutMs) {

      await Connect(host, port);

      //if (!ar.AsyncWaitHandle.WaitOne(timeoutMs))
      //  throw new TimeoutException();      
    }

    public async Task Connect(string host, int port) {

      try {

        Disconnect();

        HostName hostName = new HostName(host);

        _socket = new StreamSocket();

        _socket.Control.NoDelay = true;

        await _socket.ConnectAsync(hostName, port.ToString());
      } catch (Exception ex) {

        Disconnect();

        throw new Exception(ex.ToString());
      }
    }

    public void Disconnect() {

      if (_socket != null) {

        _socket.Dispose();

        _socket = null;
      }
    }

    public async Task Send(byte data) {

      await Send(new byte[] { data });
    }

    public async Task Send(byte[] data) {

      using (DataWriter writer = new DataWriter(_socket.OutputStream)) {

        try {

          writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
          writer.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

          writer.WriteBytes(data);

          await writer.StoreAsync();

          await writer.FlushAsync();
        } catch (Exception ex) {

          Disconnect();

          throw new Exception(ex.ToString());
        } finally {

          writer.DetachStream();
        }
      }
    }

    public uint Available {

      get {

        if (!IsConnected)
          throw new Exception("Not Connected - Cannot return the number of bytes available without an active connection.");

        using (DataReader reader = new DataReader(_socket.InputStream))
          try {

            return reader.UnconsumedBufferLength;
          } finally {

            reader.DetachStream();
          }
      }
    }

    public async Task<byte[]> ReadBytes(uint maxLength) {

      using (DataReader reader = new DataReader(_socket.InputStream)) {

        reader.InputStreamOptions = Windows.Storage.Streams.InputStreamOptions.Partial;
        reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
        reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

        uint toRead = await reader.LoadAsync(maxLength);

        if (toRead == 0)
          return new byte[] { };

        try {

          byte[] buffer = new byte[toRead];

          reader.ReadBytes(buffer);

          return buffer;
        } finally {

          reader.DetachStream();
        }
      }
    }

    public async Task<byte> ReadByte() {

      using (DataReader reader = new DataReader(_socket.InputStream)) {

        reader.InputStreamOptions = Windows.Storage.Streams.InputStreamOptions.Partial;
        reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
        reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

        try {

          uint available = await reader.LoadAsync(1);

          for (int x = 1; x < ReceiveTimeout; x++) {

            if (reader.UnconsumedBufferLength != 0)
              return reader.ReadByte();

            await Task.Delay(1);
          }

          Disconnect();

          throw new Exception("Connection attempt timed out");
        } finally {

          reader.DetachStream();
        }
      }
    }
  }
}