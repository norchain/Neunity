#if NEOSC
using Neunity.Adapters.NEO;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
#else
using Neunity.Adapters.Unity;
#endif

namespace Neunity.Tools{
	
	public static class ErDomSys
    {
		// No error domain
        public static readonly byte[] Nothing = { 0 };
        // The errors predefined in Neunity system
        public static readonly byte[] Sys = { 1 };

    }


    
	public static class SysCode
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

    public class Error{
        public byte domain;
        public byte code;
        public byte[] message;
    }

    public class Response
    {
        byte[] error;
        byte[] body;
    }


    /**
        NUTP stands for Neunity Transfer Protocol
    */
	public static class NUTP{
        public static byte[] Success() => SD.Seg(new byte[3] { 0, 0, 0 });

		public static byte[] Sys(byte[] status) => SD.Seg(Op.JoinByteArray(ErDomSys.Sys, status, new byte[1] { 0 }));
		public static byte[] SysWithMessage(byte[] status,byte[] message) => SD.Seg(Op.JoinByteArray(ErDomSys.Sys, status,message ));

		public static byte[] Code(byte[] domain, byte[] code) => SD.Seg(Op.JoinByteArray(domain, code, new byte[1] { 0 }));
		public static byte[] CodeWithMessage(byte[] domain, byte[] code, byte[] message) => SD.Seg(Op.JoinByteArray(domain, code, message));

        public static Error Bytes2Error(byte[] data) => new Error()
        {
			domain = data[0],
			code = data[1],
			message = Op.SubBytes(data, 2, data.Length - 2)
        };

		public static byte[] Error2Bytes(Error error) => new byte[0];

        public static Response Bytes2Response(byte[] data){
            Response response = new Response();
			return response;
        }
	}

}