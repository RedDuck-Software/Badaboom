﻿using Badaboom.Backend.Controllers;
using Badaboom.Backend.Infrastructure.Services;
using Badaboom.Core.Models.Request;
using Badaboom.Core.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class TransactionController : BaseController
    {
        //private readonly ApplicationDbContext context;

        //public TransactionController(ApplicationDbContext context)
        //{
        //    this.context = context;
        //}
        private ITransactionService _transactionService;


        public TransactionController(ITransactionService transactionSerivice)
        {
            _transactionService = transactionSerivice;
        }

        [HttpGet("GetContractAbi")]
        public async Task<ActionResult<string>> GetContractAbi([FromQuery] string contractAddress)
        {
            var res = await _transactionService.GetContractAbi(contractAddress);

            if (res == null) return new NotFoundResult();

            return res;
        }

        [HttpPost("GetTransactions")]
        public async Task<ActionResult<PaginationTransactionResponse>> GetFilteredTransactions([FromQuery] GetFilteredTransactionRequest request)
        {
            IEnumerable<Transaction> res;

            if (request.DecodeInputDataInfo != null)
                res = await _transactionService.GetPaginatedFilteredTransactionsWithInputParameters(request, 10);
            else
                res = await _transactionService.GetPaginatedFilteredTransactions(request);

            return new PaginationTransactionResponse()
            {
                Count = res.Count(),
                Transactions = res.ToList()
            };
        }

        [HttpGet]
        public ActionResult<PaginationTransactionResponse> Get([FromQuery] PaginationDTO pagination)
        {
            string json = @"
[
  {
    'TxnHash': '0x7deb1d3cbd5dfa1cc36fe48b5bb0ea9e18380fcf3ba9c9c827710686a963f8bb',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0x18f52ff52ca56b6d8fbd582bf4337f243fbd62183f7dd3d46711490814111478',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0x8edf36643866f9b1867bbbe67ab658077b9d83019b0bb5dc4b7c63439fa44614',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xa775157e68ee605deee2c377602f0c7d979d664e9e24e05a0b9be4ead2d39ed2',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xh457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf339',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  }
]
";

            var transactions = JsonConvert.DeserializeObject<IEnumerable<Transaction>>(json);

            int count = CountPageQuantity(transactions, pagination.quantityPerPage);

            var transactionsPerPage = transactions
                .Skip((pagination.page - 1) * pagination.quantityPerPage)
                .Take(pagination.quantityPerPage);

            PaginationTransactionResponse response = new()
            {
                Count = count,
                Transactions = transactionsPerPage
            };

            return response;
        }

        private static int CountPageQuantity(IEnumerable<Transaction> enumerable, int recordsPerPage)
        {
            int count = enumerable.Count();
            int additionalPage = count % recordsPerPage;
            int pagesQuantity = count / recordsPerPage + (additionalPage > 0 ? 1 : 0);
            return pagesQuantity;
        }
    }
}
