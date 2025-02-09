﻿using ENTITIES;
using ENTITIES.CustomModels.InternationalCollaboration.Collaboration.MemorandumOfUnderstanding.MOU;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.InternationalCollaboration.Collaboration.MemorandumOfUnderstanding
{
    public class PartnerMOURepo
    {
        readonly ScienceAndInternationalAffairsEntities db = new ScienceAndInternationalAffairsEntities();
        public List<ListMOUPartner> listAllMOUPartner(string partner_name, string nation_name, string specialization_name, int mou_id)
        {
            try
            {
                string sql_mouPartnerList =
                    @"select tb1.mou_partner_id,tb9.mou_code,tb2.partner_name,tb4.specialization_name
                        ,tb2.website, tb1.contact_point_name, tb1.contact_point_phone
                        ,tb1.contact_point_email, tb1.mou_start_date, tb7.scope_abbreviation, tb1.mou_id
                        from IA_Collaboration.MOUPartner tb1 inner join IA_Collaboration.Partner tb2
                        on tb1.partner_id = tb2.partner_id 
                        inner join IA_Collaboration.MOUPartnerSpecialization tb3 on
                        tb1.mou_partner_id = tb3.mou_partner_id 
                        inner join General.Specialization tb4 on 
                        tb3.specialization_id = tb4.specialization_id
                        inner join IA_Collaboration.PartnerScope tb5 on 
                        tb5.partner_id = tb1.partner_id
                        inner join IA_Collaboration.MOUPartnerScope tb6 on 
                        tb6.partner_scope_id = tb5.partner_scope_id
                        inner join IA_MasterData.CollaborationScope tb7 on 
                        tb7.scope_id = tb5.scope_id
                        inner join General.Country tb8 on 
                        tb8.country_id = tb2.country_id
						inner join IA_Collaboration.MOU tb9 on
						tb9.mou_id = tb1.mou_id
                        where tb1.mou_id = @mou_id
                        and tb8.country_name like '%%'
                        and tb2.partner_name like '%%'
                        and tb4.specialization_name like '%%'
                        order by tb1.mou_partner_id";
                List<ListMOUPartner> mouList = db.Database.SqlQuery<ListMOUPartner>(sql_mouPartnerList,
                    new SqlParameter("mou_id", mou_id),
                    new SqlParameter("nation_name", '%' + nation_name + '%'),
                    new SqlParameter("partner_name", '%' + partner_name + '%'),
                    new SqlParameter("specialization_name", '%' + specialization_name + '%')).ToList();
                handlingPartnerListData(mouList);
                return mouList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void handlingPartnerListData(List<ListMOUPartner> mouList)
        {
            //spe_name
            //sco_abbre
            ListMOUPartner previousItem = null;
            foreach (ListMOUPartner item in mouList.ToList())
            {
                if (previousItem == null) //first record
                {
                    previousItem = item;
                    previousItem.mou_start_date_string = item.mou_start_date.ToString("dd'/'MM'/'yyyy");
                }
                else
                {
                    if (item.mou_partner_id.Equals(previousItem.mou_partner_id))
                    {
                        if (!previousItem.specialization_name.Contains(item.specialization_name))
                        {
                            previousItem.specialization_name = previousItem.specialization_name + "," + item.specialization_name;
                        }
                        if (!previousItem.scope_abbreviation.Contains(item.scope_abbreviation))
                        {
                            previousItem.scope_abbreviation = previousItem.scope_abbreviation + "," + item.scope_abbreviation;
                        }
                        //then remove current object
                        mouList.Remove(item);
                    }
                    else
                    {
                        previousItem = item;
                    }
                }
            }
            return;
        }
        public void addMOUPartner(PartnerInfo input, int mou_id)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int partner_id_item = 0;
                    //new partner
                    if (input.partner_id == 0)
                    {
                        db.Partners.Add(new ENTITIES.Partner
                        {
                            partner_name = input.partnername_add,
                            website = input.website_add,
                            address = input.address_add,
                            country_id = 13
                        });
                        //checkpoint 2
                        db.SaveChanges();
                        ENTITIES.Partner objPartner = db.Partners.Where(x => x.partner_name == input.partnername_add).First();
                        partner_id_item = objPartner.partner_id;
                    }
                    else //old partner
                    {
                        partner_id_item = input.partner_id;
                    }
                    //add to MOUPartner via each partner of MOU
                    db.MOUPartners.Add(new ENTITIES.MOUPartner
                    {
                        mou_id = mou_id,
                        partner_id = partner_id_item,
                        mou_start_date = DateTime.ParseExact(input.sign_date_mou_add, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        contact_point_name = input.represent_add,
                        contact_point_email = input.email_add,
                        contact_point_phone = input.phone_add
                    });
                    //PartnerScope and MOUPartnerScope
                    foreach (int tokenScope in input.coop_scope_add.ToList())
                    {
                        PartnerScope objPS = db.PartnerScopes.Where(x => x.partner_id == partner_id_item && x.scope_id == tokenScope).FirstOrDefault();
                        int partner_scope_id = 0;
                        if (objPS == null)
                        {
                            db.PartnerScopes.Add(new PartnerScope
                            {
                                partner_id = partner_id_item,
                                scope_id = tokenScope,
                                reference_count = 0
                            });
                            //checkpoint 3
                            db.SaveChanges();
                            PartnerScope newObjPS = db.PartnerScopes.Where(x => x.partner_id == partner_id_item && x.scope_id == tokenScope).FirstOrDefault();
                            partner_scope_id = newObjPS.partner_scope_id;
                        }
                        else
                        {
                            objPS.reference_count += 1;
                            db.Entry(objPS).State = EntityState.Modified;
                            partner_scope_id = objPS.partner_scope_id;
                        }
                        db.MOUPartnerScopes.Add(new MOUPartnerScope
                        {
                            partner_scope_id = partner_scope_id,
                            mou_id = mou_id
                        });
                    }
                    //checkpoint 4
                    db.SaveChanges();
                    //MOUPartnerSpe
                    MOUPartner objMOUPartner = db.MOUPartners.Where(x => (x.mou_id == mou_id && x.partner_id == partner_id_item)).First();
                    foreach (int tokenSpe in input.specialization_add.ToList())
                    {
                        db.MOUPartnerSpecializations.Add(new MOUPartnerSpecialization
                        {
                            mou_partner_id = objMOUPartner.mou_partner_id,
                            specialization_id = tokenSpe,
                        });
                    }
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public void editMOUPartner(PartnerInfo input, int mou_id, int mou_partner_id)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int partner_id_item = 0;
                    //new partner
                    if (input.partner_id == 0)
                    {
                        db.Partners.Add(new ENTITIES.Partner
                        {
                            partner_name = input.partnername_add,
                            website = input.website_add,
                            address = input.address_add,
                            country_id = 13
                        });
                        //checkpoint 2
                        db.SaveChanges();
                        ENTITIES.Partner objPartner = db.Partners.Where(x => x.partner_name == input.partnername_add).First();
                        partner_id_item = objPartner.partner_id;
                    }
                    else //old partner
                    {
                        partner_id_item = input.partner_id;
                    }

                    //edit MOUPartner
                    ENTITIES.MOUPartner moup = db.MOUPartners.Where(x => x.mou_partner_id == mou_partner_id).First();
                    moup.mou_id = mou_id;
                    moup.partner_id = partner_id_item;
                    moup.mou_start_date = DateTime.ParseExact(input.sign_date_mou_add, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    moup.contact_point_name = input.represent_add;
                    moup.contact_point_email = input.email_add;
                    moup.contact_point_phone = input.phone_add;
                    db.Entry(moup).State = EntityState.Modified;
                    //checkpoint 3
                    db.SaveChanges();

                    //remove old records of MOUPartnerSpecialization.
                    db.MOUPartnerSpecializations.RemoveRange(db.MOUPartnerSpecializations.Where(x => x.mou_partner_id == mou_partner_id).ToList());
                    //checkpoint 4
                    db.SaveChanges();

                    //add new records of MOUPartnerSpecialization.
                    foreach (int tokenSpe in input.specialization_add.ToList())
                    {
                        db.MOUPartnerSpecializations.Add(new MOUPartnerSpecialization
                        {
                            mou_partner_id = mou_partner_id,
                            specialization_id = tokenSpe,
                        });
                    }
                    //checkpoint 5
                    db.SaveChanges();

                    //get old records of PartnerScope in MOU
                    string sql_old_partnerScope = @"select t1.partner_scope_id,partner_id,scope_id from IA_Collaboration.PartnerScope t1 inner join 
                        IA_Collaboration.MOUPartnerScope t2 on
                        t1.partner_scope_id = t2.partner_scope_id
                        where mou_id = @mou_id";
                    List<PartnerScope> listOldPS = db.Database.SqlQuery<PartnerScope>(sql_old_partnerScope,
                        new SqlParameter("mou_id", mou_id)).ToList();

                    //reset old records of scopes of partner.
                    foreach (PartnerScope token in listOldPS.ToList())
                    {
                        PartnerScope objPS = db.PartnerScopes.Where(x => x.partner_scope_id == token.partner_scope_id).First();
                        //decrese refer_count in PartnerScope
                        objPS.reference_count -= 1;
                        db.Entry(objPS).State = EntityState.Modified;

                        //delete record of MOUPartnerScope.
                        db.MOUPartnerScopes.Remove(db.MOUPartnerScopes.Where(x => x.partner_scope_id == token.partner_scope_id).First());
                    }

                    //add new records of scopes of partner.
                    foreach (int tokenScope in input.coop_scope_add.ToList())
                    {
                        PartnerScope objPS = db.PartnerScopes.Where(x => x.partner_id == partner_id_item && x.scope_id == tokenScope).FirstOrDefault();
                        int partner_scope_id = 0;
                        if (objPS == null) //add new to PartnerScope
                        {
                            db.PartnerScopes.Add(new PartnerScope
                            {
                                partner_id = partner_id_item,
                                scope_id = tokenScope,
                                reference_count = 0
                            });
                            //checkpoint 3
                            db.SaveChanges();
                            PartnerScope newObjPS = db.PartnerScopes.Where(x => x.partner_id == partner_id_item && x.scope_id == tokenScope).FirstOrDefault();
                            partner_scope_id = newObjPS.partner_scope_id;
                        }
                        else
                        {
                            objPS.reference_count += 1;
                            db.Entry(objPS).State = EntityState.Modified;
                            partner_scope_id = objPS.partner_scope_id;
                        }
                        db.MOUPartnerScopes.Add(new MOUPartnerScope
                        {
                            partner_scope_id = partner_scope_id,
                            mou_id = mou_id
                        });
                    }
                    //checkpoint 5
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        public void deleteMOUPartner(int mou_id, int mou_partner_id)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //get partner of moupartner deleted.
                    int partner_id_item = db.MOUPartners.Find(mou_partner_id).partner_id;

                    //get all scopes of partner in MOU.
                    string sql_old_partnerScope = @"select t1.partner_scope_id,partner_id,scope_id from IA_Collaboration.PartnerScope t1 inner join 
                        IA_Collaboration.MOUPartnerScope t2 on
                        t1.partner_scope_id = t2.partner_scope_id
                        where mou_id = @mou_id and partner_id = @partner_id";
                    List<PartnerScope> listPS = db.Database.SqlQuery<PartnerScope>(sql_old_partnerScope,
                        new SqlParameter("mou_id", mou_id),
                        new SqlParameter("partner_id", partner_id_item)).ToList();

                    foreach (PartnerScope partnerScope in listPS.ToList())
                    {
                        PartnerScope ps = db.PartnerScopes.Find(partnerScope.partner_scope_id);
                        ps.reference_count -= 1;
                        db.Entry(ps).State = EntityState.Modified;
                        db.MOUPartnerScopes.Remove(db.MOUPartnerScopes.Where(x => x.partner_scope_id == partnerScope.partner_scope_id && x.mou_id == mou_id).First());
                    }
                    //checkpoint 1
                    db.SaveChanges();

                    db.MOUPartnerSpecializations.RemoveRange(db.MOUPartnerSpecializations.Where(x => x.mou_partner_id == mou_partner_id).ToList());
                    db.MOUPartners.Remove(db.MOUPartners.Find(mou_partner_id));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public List<PartnerHistory> listMOUPartnerHistory(int mou_partner_id)
        {
            string sql_history =
                    @"select main.content,acc.full_name,main.add_time from (
                    select 
                    a1.mou_partner_id,a2.account_id,a2.add_time,
                    N'Ký kết biên bản ghi nhớ' as content
                    from IA_Collaboration.MOUPartner a1
                    inner join IA_Collaboration.MOU a2 on a1.mou_id = a2.mou_id
                    union all
                    select c2.mou_partner_id,c1.account_id,c1.add_time,
                    N'Ký kết biên bản thỏa thuận' as content from IA_Collaboration.MOA c1
                    inner join IA_Collaboration.MOUPartner c2 on c1.mou_id = c2.mou_id
                    ) as main
                    left join General.Account acc on main.account_id = acc.account_id
                    where mou_partner_id = @mou_partner_id
                    order by add_time ";
            List<PartnerHistory> historyList = db.Database.SqlQuery<PartnerHistory>(sql_history,
                new SqlParameter("mou_partner_id", mou_partner_id)).ToList();
            foreach (PartnerHistory p in historyList)
            {
                p.add_time_string = p.add_time.ToString("dd'/'MM'/'yyyy");
            }
            return historyList;
        }
        public List<ENTITIES.Partner> GetPartners(int mou_id)
        {
            try
            {
                string sql_partnerList = @"select * from IA_Collaboration.Partner
                    where partner_id not in (
                    select partner_id from IA_Collaboration.MOUPartner
                    where mou_id = @mou_id)";
                List<ENTITIES.Partner> partnerList = db.Database.SqlQuery<ENTITIES.Partner>(sql_partnerList
                    , new SqlParameter("mou_id", mou_id)).ToList();
                return partnerList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ENTITIES.Partner getMOUPartnerById(int partner_id)
        {
            try
            {
                ENTITIES.Partner p = db.Partners.Find(partner_id);
                return p;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Specialization> getPartnerMOUSpe()
        {
            try
            {
                string sql_speList = @"select * from General.Specialization";
                List<Specialization> speList = db.Database.SqlQuery<Specialization>(sql_speList).ToList();
                return speList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CollaborationScope> getPartnerMOUScope()
        {
            try
            {
                string sql_scopeList = @"select * from IA_MasterData.CollaborationScope";
                List<CollaborationScope> scopeList = db.Database.SqlQuery<CollaborationScope>(sql_scopeList).ToList();
                return scopeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ListMOUPartner getMOUPartnerDetail(int mou_partner_id)
        {
            try
            {
                string sql_mouPartnerList =
                    @"select tb1.mou_partner_id,tb2.partner_name,tb4.specialization_name
                        ,tb2.website, tb1.contact_point_name, tb1.contact_point_phone
                        ,tb1.contact_point_email, tb1.mou_start_date, tb7.scope_abbreviation, 
                        tb1.mou_id
                        from IA_Collaboration.MOUPartner tb1 inner join IA_Collaboration.Partner tb2
                        on tb1.partner_id = tb2.partner_id 
                        inner join IA_Collaboration.MOUPartnerSpecialization tb3 on
                        tb1.mou_partner_id = tb3.mou_partner_id 
                        inner join General.Specialization tb4 on 
                        tb3.specialization_id = tb4.specialization_id
                        inner join IA_Collaboration.PartnerScope tb5 on 
                        tb5.partner_id = tb1.partner_id
                        inner join IA_Collaboration.MOUPartnerScope tb6 on 
                        tb6.partner_scope_id = tb5.partner_scope_id
                        inner join IA_MasterData.CollaborationScope tb7 on 
                        tb7.scope_id = tb5.scope_id
                        inner join General.Country tb8 on 
                        tb8.country_id = tb2.country_id
                        where tb1.mou_partner_id = @mou_partner_id";
                List<ListMOUPartner> mouList = db.Database.SqlQuery<ListMOUPartner>(sql_mouPartnerList,
                    new SqlParameter("mou_partner_id", mou_partner_id)).ToList();
                handlingPartnerListData(mouList);
                ListMOUPartner obj = mouList.First();
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public class ListMOUPartner
        {
            public ListMOUPartner() { }
            public string mou_code { get; set; }
            public int mou_partner_id { get; set; }
            public int mou_id { get; set; }
            public string partner_name { get; set; }
            public string country_name { get; set; }
            public string specialization_name { get; set; }
            public string specialization_abbreviation { get; set; }
            public string website { get; set; }
            public string contact_point_name { get; set; }
            public string contact_point_phone { get; set; }
            public string contact_point_email { get; set; }
            public string mou_start_date_string { get; set; }
            public DateTime mou_start_date { get; set; }
            public string scope_abbreviation { get; set; }
        }
        public class MOUPartnerAdd : ListMOUPartner
        {
            public string address { get; set; }
            public int partner_id { get; set; }
            public List<int> list_spe { get; set; }
            public List<int> list_scope { get; set; }
        }
        public class PartnerHistory
        {
            public string full_name { get; set; }
            public string content { get; set; }
            public DateTime add_time { get; set; }
            public string add_time_string { get; set; }
        }
    }
}
