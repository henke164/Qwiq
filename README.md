# Qwiq <img src="https://github.com/henke164/Qwiq/blob/master/icon.png" alt="" width="50" />

Qwiq is an open source quick in-memory data store, used as a cache broker.

Qwiq solves the problem where you are in need of larger number of data from the cache broker at the same time where normal requests such as HTTP aren't as optimized as you wish.

With Qwiq, you dont send and receive stored data between the Client application and Qwiq cache. Instead pointer adresses are sent from Qwiq cache, so the Client can read the data
directly from the allocated memory in the CPU.

HTTP is used to send and receive pointers between the Client and Qwiq.

Since the client is reading from another process, it needs to be runned with Administrator rights.

### Store data

```c#
var client = new Qwiq.QwiqClient(port: 1988);
await client.Connect();
await client.AddAsync("my-key", veryBigList);
```
**Under the hood:**

The client will break down myObject into bytes and tell Qwiq to allocate that number of bytes and connect the allocated memory address to the specific KEY.

Qwiq returns the address of that allocated space and the Client can write that object into Qwiq's process memory.

### Reading data

```c#
var client = new Qwiq.QwiqClient(port: 1988);
await client.Connect();
var veryBigList = await client.GetAsync<List<MyObject>>("my-key");
```
**Under the hood:**

Client asks Qwiq what the pointeraddress is for "my-key". Client will get the pointer address and the size in bytes for that object. When the bytes are read from Qwiq memory,
the client will build (MyObject)myObject from the bytes.

### Speed test result with 100 000 objects
<img src="https://github.com/henke164/Qwiq/blob/master/ExampleApp/timing.png" alt="" width="800" />
