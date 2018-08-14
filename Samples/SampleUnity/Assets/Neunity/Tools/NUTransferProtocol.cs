#if NEOSC
using Neunity.Adapters.NEO;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
#else
using Neunity.Adapters.Unity;
#endif
using System.Numerics;



/**
 * -------------------------------------------------
 *    NuTP stands for Neunity Transfer Protocol
 * -------------------------------------------------
    Response:
        The object Response is a standardized return value of SC invoke. A Response contains 
        error <header> and content <body>. The NuSD definition of Response is:
        <response> = [S<header>,<body>] 
    Header:
        NuSD definition: 
        <header> = [<domain#1>,<code#1>,<desp>]
        
        - If <domain#1> == NuTP.SysDom, it's a system response. We have predefined HTTP-like 
        error codes in NuTP.Code. Follow these definitions can enhance the experience if the SC
        is shared with other dynamic SCs or clients
        - It's suggested that project can define customized error codes with other <domain#1> values 
    Body:
        NuSD doesn't have specific requirement for Body. It can be a NuSD segment, table, or any structure.

    NOTE: The implementation is actually using 
        <header> = [S<domain#1>,S<code#1>,S<desp>]
        <response> = [S<header>,S<body>]
        This is for the convience for now. Will do the optimization with the above NuSD standard in near future
    
*/

namespace Neunity.Tools{
    
    
	public static class NuTP{
		public static readonly byte[] SysDom = { 0 };


		public static class Code
        {

            /** 
             * Refer to HTTP 200
            */
            public static readonly byte[] Success = { 0 };

            /** 
             * The 40 status code, or Bad Request error, means the request that was sent to 
             * the SC has invalid syntax.
             * Refer to HTTP 400
            */
            public static readonly byte[] BadRequest = { 40 };

            /**
             * The 41 status code, or an Unauthorized error, means that the user trying to 
             * access the SC operation has not been authenticated or has not been authenticated correctly. 
             * This means that the user must provide credentials to be able to view the protected resource.
             * Refer to HTTP 401
            */
            public static readonly byte[] Unauthorized = { 41 };

            /**
             * The 43 status code, or a Forbidden error, means that the user made a valid request but the 
             * SC is refusing to serve the request, due to a lack of permission to access the requested 
             * resource. 
             * Refer to HTTP 403
            */
            public static readonly byte[] Forbidden = { 43 };

            /**
             * The 44 status code, or a Not Found error, means that the user is able to communicate with 
             * the SC but it is unable to locate the requested operation.
             * Refer to HTTP 404
            */
            public static readonly byte[] NotFound = { 44 };

            /** 
             * The 50 status code, or Internal Server Error, means that SC cannot process the request 
             * for an unknown reason. Sometimes this code will appear when more specific 5x errors are 
             * more appropriate.
             * Refer to HTTP 500
            */
            public static readonly byte[] InternalServerError = { 50 };

            /**
             * The 52 status code, or Bad Gateway error, means that the SC is a gateway or proxy SC,
             * and it is not receiving a valid response from the dynamic called SC that should actually 
             * fulfill the request. 
             * This is useful when a SC is dynamically invoking another SC but getting invalid response
             * Refer to HTTP 500
            */
            public static readonly byte[] BadGateway = { 52 };


            /** Reserved. No clear meaning for now ...
             * Refer to HTTP 503
            */
            public static readonly byte[] ServiceUnavailable = { 53 };

            /** Reserved. No clear meaning for now ...
             * Refer to HTTP 504
            */
            public static readonly byte[] GatewayTimeout = { 54 };

        }

        public static readonly Header headerSuccess = new Header
        {
            domain = SysDom,
            code = Code.Success
        };

        public static Header HeaderWithCode(byte[] code) => new Header
        {
            domain = SysDom,
            code = code
        };

        public static Header HeaderWithDesp(byte[] code, byte[] description) => new Header
        {
            domain = SysDom,
            code = code,
            description = description
        };

        public static Header HeaderWithDomain( byte[] domain, byte[] code) => new Header
        {
            domain = domain,
            code = code
        };

        public static Header HeaderWithDomainDesp(byte[] domain, byte[] code, byte[] description) => new Header
        {
            domain = domain,
            code = code,
            description = description
        };

        public static Response ResWithHeader(Header header) => new Response
        {
            header = header
        };

        public static byte[] RespDataWithDetail(byte[] domain, byte[]code, byte[] description, byte[] body ) => NuSD.Seg(Header2Bytes(HeaderWithDomainDesp(domain, code, description)))
                                                                                                                    .AddSeg(body);

        public static byte[] RespDataWithCode(byte[] domain, byte[] code) => NuSD.Seg(Header2Bytes(HeaderWithDomainDesp(domain, code, Op.Void)));

        public static byte[] RespDataSucWithBody(byte[] body) => RespDataWithDetail(SysDom, Code.Success, Op.Void, body);

        public static byte[] RespDataSuccess() => RespDataWithDetail(SysDom, Code.Success, Op.Void, Op.Void);
  
		public static Header Bytes2Header(byte[] data) => new Header
        {
            domain = data.SplitTbl(0),
            code = data.SplitTbl(1),
            description = data.SplitTbl(2)
            //domain = Op.SubBytes(data, 0, 1),
            //code = Op.SubBytes(data, 1, 1),
            //description = data.SplitBody(2, data.Length - 2)
        };

        //public static byte[] Header2Bytes(Header error) => SD.JoinSegs2Table(
        //         Op.SubBytes(error.domain, 0, 1),
        //         Op.SubBytes(error.code, 0, 1),
        //SD.Seg(error.description)
        //);
        public static byte[] Header2Bytes(Header header) => NuSD.Seg(header.domain)
                                                                .AddSeg(header.code)
                                                                .AddSeg(header.description);

        public static Response Bytes2Response(byte[] data) => new Response
        {
			//header = Bytes2Header(SD.DesegWithIdFromTable(data, 0)),
            header = Bytes2Header(data.SplitTbl(0)),
            body = data.SplitTbl(1)
            //body = SD.DesegWithIdFromTable(data, 1)
        };

        //     public static byte[] Response2Bytes(Response response) => SD.JoinSegs2Table(
        //Header2Bytes(response.header),
        //    SD.Seg(response.body)
        //);
        public static byte[] Response2Bytes(Response response) => NuSD.Seg(Header2Bytes(response.header))
                                                                    .AddSeg(response.body);

		public class Header
        {
            public byte[] domain;   // Only Allow 1 byte
            public byte[] code;     //Only Allow 1 byte
            public byte[] description;  //Can be multiple bytes
        }

        public class Response
        {
            public Header header;
            public byte[] body;
        }
	}   
}