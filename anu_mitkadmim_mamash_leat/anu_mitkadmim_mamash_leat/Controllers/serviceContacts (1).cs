using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using anu_mitkadmim_mamash_leat.Models;
using anu_mitkadmim_mamash_leat.Data;

namespace anu_mitkadmim_mamash_leat.Controllers
{
    [Authorize]
    public class serviceContacts : Controller
    {
        private readonly anu_mitkadmim_mamash_leatContext _context;
        public serviceContacts(anu_mitkadmim_mamash_leatContext context)
        {
            _context = context;
        }

        // GET: Contacts


        public async Task<List<Contact>> getall(string a)
        {
            var res2 = await (from d in _context.Contact where d.userid == a select d).ToListAsync();
            //return Json(await _context.Contact.ToListAsync());

            return res2;
        }

        // GET: Contacts/Details/5
        public async Task<JsonResult> getby(string id, string a)
        {
            if (id == null || _context.Contact == null)
            {
                return null;
            }

            var contact = await _context.Contact.FirstOrDefaultAsync(m => m.idname == id && m.userid == a);
            if (contact == null)
            {
                return null;
            }

            return Json(contact);
        }

        // POST: Contacts/Create
        public async Task<IActionResult> add(string id, string name, string server, string a)
        {
            if (ModelState.IsValid)
            {

                Contact contact = new Contact();
                contact.idname = id;
                contact.last = null;
                contact.server = server;
                contact.userid = a;
                contact.lastdate = null;
                contact.nickName = name;
                contact.idmassage = null;
                _context.Contact.Add(contact);
                await _context.SaveChangesAsync();
                return Created(string.Format("/api/Contacts/{0}", contact.id), contact);
            }
            return BadRequest();
        }

        // POST: Contacts/Edit/5
        public async Task<IActionResult> edit(string name, string server, string id, string a)
        {
            var contact = await (from d in _context.Contact where d.idname == id && d.userid == a select d).ToListAsync();
            if (contact.Count == 0)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    contact.First().nickName = name;
                    contact.First().server = server;
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.First().idname))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();
            }
            return BadRequest();
        }

        // delete: Contacts/Delete/5
        public async Task<IActionResult> deletecon(string id, string a)
        {
            var contact = await _context.Contact.FindAsync(id);
            if (contact != null)
            {
                var cc = await (from c in _context.Message where c.user1 == a && c.user2 == id select c).ToListAsync();
                if (cc.Count != 0)
                {
                    foreach (Message aa in cc)
                    {
                        _context.Message.Remove(aa);
                    }
                }
                cc = await (from c in _context.Message where c.user2 == a && c.user1 == id select c).ToListAsync();
                if (cc.Count != 0)
                {
                    foreach (Message aa in cc)
                    {
                        _context.Message.Remove(aa);
                    }
                }
                _context.Contact.Remove(contact);
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        private bool ContactExists(string id)
        {
            return _context.Contact.Any(e => e.idname == id);
        }

        // GET: Contacts/:id/messages
        public async Task<IActionResult> chatWith(string id, string a)
        {
            if (id == null || _context.Contact == null)
            {
                return NotFound();
            }
            var cid = await (from c in _context.Message where c.user1 == a && c.user2 == id select c).ToListAsync();
            bool flag = false;
            if (cid.Count() != 0)
            {
                flag = true;
            }
            var cid2 = await (from c in _context.Message where c.user2 == a && c.user1 == id select c).ToListAsync();
            if (cid2.Count() != 0)
            {
                flag = true;
            }
            if (!flag)
            {
                return BadRequest();
            }
            cid.AddRange(cid2);
            return Json(cid);
        }

        // POST: Contacts/:id/messages
        public async Task<IActionResult> CreateMess(string id, string content, string a)
        {
            var conlist = await (from c in _context.Contact where c.idname == id && c.userid == a select c).ToListAsync();
            if(conlist.Count() == 0)
            {
                return BadRequest();
            }
            var con = conlist.First();
            if (con.server == "localhost:6132")
            {
                if (ModelState.IsValid)
                {
                    Message message = new Message();
                    message.created = DateTime.Now.ToString();
                    message.user1 = a;
                    message.user2 = id;
                    message.content = content;
                    message.sent = true;
                    _context.Message.Add(message);
                    await _context.SaveChangesAsync();
                    con.lastdate = DateTime.Now.ToString();
                    con.last = content;
                    con.idmassage = message.id;
                    _context.Update(con);
                    await _context.SaveChangesAsync();
                    var other = await (from c in _context.Contact where c.idname == a && c.userid == id select c).ToListAsync();
                    other.First().lastdate = DateTime.Now.ToString();
                    other.First().last = content;
                    other.First().idmassage = message.id;
                    _context.Update(other.First());
                    await _context.SaveChangesAsync();
                    return Created(string.Format("/api/Contacts/{0}/messages/{1}", id, message.id), message);
                }
                return BadRequest();
            }
            else
                return Ok(con);
        }

        // GET: Contacts/:id/messages/:id2
        public async Task<IActionResult> getMessage(string id, int id2, string a)
        {
            var mess = await (from d in _context.Message where d.id == id2 && d.user2 == a select d).ToListAsync();
            if (mess.Count == 0)
            {
                return NotFound();
            }
            return Json(mess);
        }

        // put: Contacts/:id/messages/:id2
        public async Task<IActionResult> putMessage(string id, int id2, string content, string a)
        {
            var mess = await (from d in _context.Message where d.id == id2 select d).ToListAsync();
            if (mess.Count() == 0)
            {
                return NotFound();
            }
            var outmess = mess.First();
            outmess.content = content;
            outmess.created = DateTime.Now.ToString();
            await _context.SaveChangesAsync();

            var contact_id = await (from c in _context.Message where c.id == id2 && c.sent == true select c.user1).ToListAsync();
            if(contact_id.Count() == 0)
            {
                contact_id = await (from c in _context.Message where c.id == id2 && c.sent == false select c.user1).ToListAsync();
                if (contact_id.Count() == 0)
                {
                    return BadRequest();
                }
            }
            var con = await (from c in _context.Contact where c.idname == contact_id.First() select c).ToListAsync();
            con.First().lastdate = DateTime.Now.ToString();
            con.First().last = content;
            _context.Update(con);
            await _context.SaveChangesAsync();
            return Json(outmess);
        }
        public async Task<IActionResult> DeleteMess(string id, int id2, string a)
        {
            var outmes = await (from e in _context.Message where e.id == id2 select e).ToListAsync();
            if (outmes.Count() == 0)
            {
                return NotFound();
            }
            var outmess = outmes.First();
            _context.Message.Remove(outmess);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    //class nosaf
}