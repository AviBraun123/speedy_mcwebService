using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using anu_mitkadmim_mamash_leat.Data;
using anu_mitkadmim_mamash_leat.Models;
using anu_mitkadmim_mamash_leat.Controllers;
using anu_mitkadmim_mamash_leat.hub;
using Microsoft.AspNetCore.SignalR;

namespace anu_mitkadmim_mamash_leat.Controllers
{
    [Authorize]
    public class indipandesService : Controller
    {
        private readonly anu_mitkadmim_mamash_leatContext _context;
        private readonly IHubContext<Class> hub;

        public indipandesService(anu_mitkadmim_mamash_leatContext context, IHubContext<Class> _hub)
        {
            hub = _hub;
            _context = context;
        }
        // POST: Contacts/:id/messages
        public async Task<IActionResult> invitations(string From, string to, string server)
        {
            if (server != "localhost:6132")
            {
                var userFrom = await (from c in _context.User where c.id == From select c).ToListAsync();
                if (userFrom.Count() == 0)
                {
                    return NotFound();
                }
                var con = await (from c in _context.Contact where c.idname == to select c).ToListAsync();
                if (con.Count() == 0)
                {
                    Contact contactTo = new Contact();
                    contactTo.userid = From;
                    contactTo.server = server;
                    contactTo.lastdate = null;
                    contactTo.idname = to;
                    contactTo.nickName = to;
                    contactTo.last = null;
                    contactTo.idmassage = null;
                    _context.Contact.Add(contactTo);
                    await _context.SaveChangesAsync();
                }
                await hub.Clients.All.SendAsync("recive_contact: " + to, new { contact_id = From, server = "localhost:6132" });
                return NoContent();
            }
            else
            {
                return Ok();
            }
        }

        public async Task<IActionResult> transfer(string From, string to, string content)
        {
            var conlist = await (from c in _context.Contact where c.idname == to select c).ToListAsync();
            if (conlist.Count() == 0)
            {
                return BadRequest();
            }
            var con = conlist.First();
            if (con.server != "localhost:6132")
            {
                var userFrom = await (from c in _context.User where c.id == From select c).ToListAsync();
                if (userFrom.Count == 0)
                {
                    return NotFound();
                }
                var contact = await (from c in _context.Contact where c.idname == to select c).ToListAsync();
                if (contact.Count == 0)
                {
                    return NotFound();
                }
                string a = DateTime.Now.ToString() ;
                Message message = new Message();
                message.content = content;
                message.created = a;
                message.sent = true;
                message.user1 = From;
                message.user2 = to; 
                _context.Message.Add(message);
                await _context.SaveChangesAsync();
                contact.First().idmassage = message.id;
                contact.First().last = content;
                contact.First().lastdate = a;
                _context.Contact.Update(contact.First());
                await _context.SaveChangesAsync();
                await hub.Clients.All.SendAsync("recive_message: " + to, new { contact_id = From, ContentMessege = content, time = DateTime.Now, sent = false,});
                return NoContent();
            }
            else
                return Ok(con);
        }
    }
}