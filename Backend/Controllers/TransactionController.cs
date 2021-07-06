using ClientCore.Models;
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
    public class TransactionController : ControllerBase
    {
        //private readonly ApplicationDbContext context;

        //public TransactionController(ApplicationDbContext context)
        //{
        //    this.context = context;
        //}

        [HttpGet/*("transaction")*/]
        //[AllowAnonymous]
        public async Task<ActionResult<PaginationTransactionResponse>> Get([FromQuery] PaginationDTO pagination)
        {
            string json = @"
[
  {
    'TxnHash': '0xf457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf337',
    'Method': 'Transfer',
    'Block': 12767223,
    'Age': 1625485471,
    'From': '0xc4ffca4a2e9fcf7a120c52cd06f755e850a04c72',
    'To': '0x8596cd338c08b43644c1727536d31e0139e581ca',
    'Value': 0.29891693,
    'TxnFee': 0.0003003
  },
  {
    'TxnHash': '0xg457f48cb12957d4df533c1eb0e6a086035239b353d52e2f1eb28a60751bf338',
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
