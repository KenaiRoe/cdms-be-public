﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NLog;
using services.Models;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json.Linq;
using services.Resources;

namespace services.Controllers
{
    /* 
     * Handles all dataset related api calls
     */
    [System.Web.Http.Authorize]
    public class DatasetController : CDMSController
    {

        public IEnumerable<Dataset> GetDatasets()
        {
            var db = ServicesContext.Current;

            var datasets = db.Datasets.Include(d => d.DefaultRowQAStatus);
            return datasets.AsEnumerable();
        }

        // GET /api/v1/dataset/getdataset/5
        public Dataset GetDataset(int id)
        {
            var db = ServicesContext.Current;

            Dataset dataset = db.Datasets.Find(id);
            if (dataset == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return dataset;
        }

        // POST /api/v1/dataset/updatedataset
        [HttpPost]
        public HttpResponseMessage UpdateDataset(JObject jsonData)
        {
            var db = ServicesContext.Current;

            dynamic json = jsonData;

            Dataset ds = db.Datasets.Find(json.dataset.Id.ToObject<int>());

            ds.Name = json.dataset.Name;
            ds.Description = json.dataset.Description;
            ds.DefaultActivityQAStatusId = json.dataset.DefaultActivityQAStatusId.ToObject<int>();
            ds.DefaultRowQAStatusId = json.dataset.DefaultRowQAStatusId.ToObject<int>();
            ds.Config = json.dataset.Config;


            db.Entry(ds).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

        public HttpResponseMessage PostDataset(Dataset dataset)
        {
            var db = ServicesContext.Current;

            if (ModelState.IsValid)
            {
                db.Datasets.Add(dataset);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, dataset);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = dataset.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        public HttpResponseMessage DeleteDataset(int id)
        {
            var db = ServicesContext.Current;

            Dataset dataset = db.Datasets.Find(id);
            if (dataset == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Datasets.Remove(dataset);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, dataset);
        }

        protected override void Dispose(bool disposing)
        {
            var db = ServicesContext.Current;

            db.Dispose();
            base.Dispose(disposing);
        }

        
        // GET /api/v1/dataset/getheadersdatafordataset/5
        public DataTable GetHeadersDataForDataset(int id)
        {
            var db = ServicesContext.Current;
            Dataset dataset = db.Datasets.Find(id);
            if (dataset == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            string query = "SELECT h.* FROM " + dataset.Datastore.TablePrefix + "_Header_VW h JOIN Activities a on a.Id = h.ActivityId WHERE a.DatasetId = " + dataset.Id;
            //logger.Debug("query = " + query);

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ServicesContext"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }

        //api/v1/dataset/adddatasettoproject
        [HttpPost]
        public HttpResponseMessage AddDatasetToProject(JObject jsonData)
        {
            logger.Debug("Here we are adding a dataset to a project!");
            var db = ServicesContext.Current;

            dynamic json = jsonData;
            int DatastoreId = json.DatastoreId.ToObject<int>();
            Datastore ds = db.Datastores.Find(DatastoreId);

            int ProjectId = json.ProjectId.ToObject<int>();
            Project p = db.Projects.Find(ProjectId);

            logger.Debug("We have a datastore to save: " + ds.Name + " for Project: " + p.Name);

            //CREATE A TRANSACTION!

            Dataset new_ds = new Dataset();
            new_ds.ProjectId = p.Id;
            new_ds.Name = p.Name + "-"+ds.Name;
            new_ds.DefaultRowQAStatusId = 1;
            new_ds.StatusId = 1;
            new_ds.CreateDateTime = DateTime.Now;
            new_ds.DefaultActivityQAStatusId = 6;
            new_ds.DatastoreId = ds.Id;
            new_ds.Config = null;

            QAStatus qa1 = db.QAStatuses.Find(5);
            QAStatus qa2 = db.QAStatuses.Find(6);
            QAStatus qa3 = db.QAStatuses.Find(1);

            new_ds.QAStatuses = new List<QAStatus>();
            new_ds.QAStatuses.Add(qa1);
            new_ds.QAStatuses.Add(qa2);

            new_ds.RowQAStatuses = new List<QAStatus>();
            new_ds.RowQAStatuses.Add(qa3);

            new_ds.Fields= new List<DatasetField>();

            foreach (var field in json.DatasetFields)
            {
                logger.Debug(" -- " + field.Id); 
                int field_id = field.Id.ToObject<int>();
                Field the_field = db.Fields.Find(field_id);

                DatasetField the_ds_field = new DatasetField();

                the_ds_field.FieldId = the_field.Id;
                the_ds_field.FieldRoleId = 2;
                the_ds_field.CreateDateTime = DateTime.Now;
                the_ds_field.Label = the_field.Name;
                the_ds_field.DbColumnName = the_field.DbColumnName;
                the_ds_field.ControlType = the_field.ControlType;
                the_ds_field.Validation = the_field.Validation;
                the_ds_field.Rule = the_field.Rule;
                the_ds_field.SourceId = 1;


                new_ds.Fields.Add(the_ds_field);
                //logger.Debug(" -- added " + field.Id);

            }

            db.Datasets.Add(new_ds);
            db.SaveChanges();

            //logger.Debug(new_ds);
            
            return new HttpResponseMessage(HttpStatusCode.OK);

        }


        //TODO: this is a (currently) unused feature we will want to revisit to support NESTED DATASETS
        // GET /api/v1/dataset/getrelationdata
        [HttpPost]
        public DataTable GetRelationData(JObject jsonData)
        {
            //int FieldId, int ActivityId, int ParentRowId)
            var db = ServicesContext.Current;
            dynamic json = jsonData;
            int FieldId = json.FieldId.ToObject<int>();

            Field f = db.Fields.Find(FieldId);

            if (f == null || f.ControlType != "grid")
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            DataTable dt = new DataTable();

            if (json["ActivityId"] is JToken && json["ParentRowId"] is JToken)
            {

                int DatasetId = Convert.ToInt32(f.DataSource);
                Dataset dataset = db.Datasets.Find(DatasetId);

                if (dataset == null)
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

                int ActivityId = json.ActivityId.ToObject<int>();
                int ParentRowId = json.ParentRowId.ToObject<int>();

                //dbcolumname for "grid" type fields 
                string query = "SELECT h.* FROM " + dataset.Datastore.TablePrefix + "_" + f.DbColumnName + "_VW h WHERE h.ActivityId = " + ActivityId + " AND h.ParentRowId = " + ParentRowId;

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ServicesContext"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // POST /api/v1/dataset/savedatasetfield
        [HttpPost]
        public HttpResponseMessage SaveDatasetField(JObject jsonData)
        {
            var db = ServicesContext.Current;

            dynamic json = jsonData;

            User me = AuthorizationManager.getCurrentUser();

            DatasetField df = db.DatasetFields.Find(json.Id.ToObject<int>());

            if (df == null || me == null)
                throw new System.Exception("Configuration error. Please try again.");

            df.Label = json.Label;
            df.Validation = json.Validation;
            df.Rule = json.Rule;
            df.FieldRoleId = json.FieldRoleId.ToObject<int>();
            try
            {
                df.OrderIndex = json.OrderIndex.ToObject<int>();
            }
            catch (System.Exception)
            {
                logger.Debug("didn't have an orderindex.");
            }
            df.ControlType = json.ControlType;
            df.SourceId = json.SourceId.ToObject<int>();

            db.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.OK);


        }

        /*
         * Remove a field from a project's version of the dataset
         * (does not remove the master field).
         */
         // POST /api/v1/dataset/deletedatasetfield
        [HttpPost]
        public HttpResponseMessage DeleteDatasetField(JObject jsonData)
        {
            logger.Debug("Inside DatastoreController.cs, DeleteDatasetField...");
            var db = ServicesContext.Current;
            dynamic json = jsonData;
            //logger.Debug("json = " + json);

            int DatasetId = json.DatasetId.ToObject<int>();
            logger.Debug("DatasetId = " + DatasetId);
            var dataset = db.Datasets.Find(DatasetId);
            if (dataset == null)
                throw new System.Exception("Dataset could not be found: " + DatasetId);

            int FieldId = json.FieldId.ToObject<int>();
            logger.Debug("FieldId = " + FieldId);

            //var field = db.DatasetFields.Find(FieldId); // Original line
            List<DatasetField> datasetFieldRecords = (from item in db.DatasetFields
                                                      where item.DatasetId == DatasetId && item.FieldId == FieldId
                                                      select item).ToList();

            if (datasetFieldRecords.Count == 0)
                throw new System.Exception("Field could not be retrieved for dataset: " + DatasetId);
            else if (datasetFieldRecords.Count > 1)
                logger.Debug("Found " + datasetFieldRecords.Count + " records (fields).");
            else
                logger.Debug("Found field.");

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ServicesContext"].ConnectionString))
            {
                con.Open();

                //var query = "DELETE FROM DatasetFields where DatasetId = " + dataset.Id + " and FieldId = " + field.Id; // Original line
                var query = "";
                foreach (var item in datasetFieldRecords)
                {
                    query = "DELETE FROM DatasetFields where Id = " + item.Id;
                    logger.Debug("SQL command = " + query);
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        //logger.Debug(query);
                        //cmd.ExecuteNonQuery(); // Original line
                        try
                        {
                            cmd.ExecuteNonQuery();
                            logger.Debug("Delete action done...");
                        }
                        catch (System.Exception e)
                        {
                            logger.Debug("Delete action failed:  " + e.Message);
                        }
                    }
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

    }
}