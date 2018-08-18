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
        <header> = [<domain>,<code>,<desp>]
        
        - If <domain#1> == NuTP.SysDom, it's a system response. We have predefined HTTP-like 
        error codes in NuTP.Code. Follow these definitions can enhance the experience if the SC
        is shared with other dynamic SCs or clients
        - It's suggested that project can define customized error codes with other <domain#1> values 
    Body:
        NuSD doesn't have specific requirement for Body. It can be a NuSD segment, table, or any structure.

    NOTE: The implementation is actually using 
        <header> = [S<domain>,S<code>,S<desp>]
        <response> = [S<header>,S<body>]
        This is for the convience for now. Will do the optimization with the above NuSD standard in near future
    
*/

namespace Neunity.Tools{
    
    
	public static class NuTP{
        public static readonly BigInteger SysDom = 0;


		public static class Code
        {

            /** 
             * Refer to HTTP 200
            */
            public static readonly BigInteger Success = 200;

            /** 
             * The 400 status code, or Bad Request error, means the request that was sent to 
             * the SC has invalid syntax.
             * Refer to HTTP 400
            */
            public static readonly BigInteger BadRequest = 400;

            /**
             * The 41 status code, or an Unauthorized error, means that the user trying to 
             * access the SC operation has not been authenticated or has not been authenticated correctly. 
             * This means that the user must provide credentials to be able to view the protected resource.
             * Refer to HTTP 401
            */
            public static readonly BigInteger Unauthorized = 401;

            /**
             * The 403 status code, or a Forbidden error, means that the user made a valid request but the 
             * SC is refusing to serve the request, due to a lack of permission to access the requested 
             * resource. 
             * Refer to HTTP 403
            */
            public static readonly BigInteger Forbidden = 403;

            /**
             * The 404 status code, or a Not Found error, means that the user is able to communicate with 
             * the SC but it is unable to locate the requested operation.
             * Refer to HTTP 404
            */
            public static readonly BigInteger NotFound = 404;

            /** 
             * The 500 status code, or Internal Server Error, means that SC cannot process the request 
             * for an unknown reason. Sometimes this code will appear when more specific 5x errors are 
             * more appropriate.
             * Refer to HTTP 500
            */
            public static readonly BigInteger InternalServerError = 500;

            /**
             * The 502 status code, or Bad Gateway error, means that the SC is a gateway or proxy SC,
             * and it is not receiving a valid response from the dynamic called SC that should actually 
             * fulfill the request. 
             * This is useful when a SC is dynamically invoking another SC but getting invalid response
             * Refer to HTTP 502
            */
            public static readonly BigInteger BadGateway = 502;


            /** Reserved. No clear meaning for now ...
             * Refer to HTTP 503
            */
            public static readonly BigInteger ServiceUnavailable = 503;

            /** Reserved. No clear meaning for now ...
             * Refer to HTTP 504
            */
            public static readonly BigInteger GatewayTimeout = 504;

        }

        public static Header headerSuccess => new Header
        {
            domain = SysDom,
            code = Code.Success
        };

        public static Header HeaderWithCode(BigInteger code) => new Header
        {
            domain = SysDom,
            code = code
        };

        public static Header HeaderWithDesp(BigInteger code, string description) => new Header
        {
            domain = SysDom,
            code = code,
            description = description
        };

        public static Header HeaderWithDomain( BigInteger domain, BigInteger code) => new Header
        {
            domain = domain,
            code = code
        };

        public static Header HeaderWithDomainDesp(BigInteger domain, BigInteger code, string description) => new Header
        {
            domain = domain,
            code = code,
            description = description
        };

        public static Response ResWithHeader(Header header) => new Response
        {
            header = header
        };

        public static byte[] RespDataWithDetail(BigInteger domain, BigInteger code, string description, byte[] body ) => NuSD.Seg(Header2Bytes(HeaderWithDomainDesp(domain, code, description)))
                                                                                                                    .AddSeg(body);

        public static byte[] RespDataWithCode(BigInteger domain, BigInteger code) => NuSD.Seg(Header2Bytes(HeaderWithDomainDesp(domain, code, "")));

        public static byte[] RespDataSucWithBody(byte[] body) => RespDataWithDetail(SysDom, Code.Success, "", body);

        public static byte[] RespDataSuccess() => RespDataWithDetail(SysDom, Code.Success, "", Op.Void());
  

		public static Header Bytes2Header(byte[] data) => new Header
        {
            domain = data.SplitTblInt(0),
            code = data.SplitTblInt(1),
            description = data.SplitTblStr(2)
        };


        public static byte[] Header2Bytes(Header header) => NuSD.SegInt(header.domain)
                                                                .AddSegInt(header.code)
                                                                .AddSegStr(header.description);

        public static Response Bytes2Response(byte[] data) => new Response
        {
            header = Bytes2Header(data.SplitTbl(0)),
            body = data.SplitTbl(1)
        };

        public static byte[] Response2Bytes(Response response) => NuSD.Seg(Header2Bytes(response.header))
                                                                    .AddSeg(response.body);

		public class Header
        {
            public BigInteger domain;   // Only Allow 1 byte
            public BigInteger code;     //Only Allow 1 byte
            public string description;  //Can be multiple bytes
        }

        public class Response
        {
            public Header header;
            public byte[] body;
        }

	}   
}