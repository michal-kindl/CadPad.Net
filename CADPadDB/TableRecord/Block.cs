﻿using System;
using System.Collections.Generic;
using System.Xml;
using CADPadDB.CADEntity;

namespace CADPadDB.TableRecord
{
    public class Block : DBTableRecord, IEnumerable<Entity>
    {

        public override string ClassName
        {
            get { return "Block"; }
        }

        private List<Entity> _items = new List<Entity>();


        public override object Clone()
        {
            Block block = base.Clone() as Block;
            foreach (Entity item in _items)
            {
                Entity itemCopy = item.Clone() as Entity;
                block.AppendEntity(itemCopy);
            }

            return block;
        }

        protected override DBObject CreateInstance()
        {
            return new Block();
        }


        public ObjectId AppendEntity(Entity entity)
        {
            if (entity.id != ObjectId.Null)
            {
                //throw new System.Exception("entity is not newly created");
            }
            return _AppendEntity(entity);
        }

        private ObjectId _AppendEntity(Entity entity)
        {
            if (_id == ObjectId.Null)
            {
                _items.Add(entity);
                entity.SetParent(this);
            }
            else
            {
                _items.Add(entity);
                entity.SetParent(this);
                this.database.IdentifyObject(entity);
            }

            return entity.id;
        }


        internal void RemoveEntity(Entity entity)
        {
            _items.Remove(entity);
        }


        internal void Clear()
        {
            _items.Clear();
        }


        public override void XmlOut(Filer.XmlFiler filer)
        {
            Filer.XmlCADDatabase filerImpl = filer as Filer.XmlCADDatabase;

            //
            base.XmlOut(filer);

            //
            filerImpl.NewSubNodeAndInsert("entities");
            foreach (Entity item in _items)
            {
                filerImpl.NewSubNodeAndInsert(item.ClassName);
                item.XmlOut(filer);
                filerImpl.Pop();
            }
            filerImpl.Pop();
        }


        public override void XmlIn(Filer.XmlFiler filer)
        {
            Filer.XmlCADDatabase filerImpl = filer as Filer.XmlCADDatabase;

            base.XmlIn(filerImpl);

            XmlNode curXmlNode = filerImpl.curXmlNode;
            XmlNode entitiesNode = curXmlNode.SelectSingleNode("entities");
            if (entitiesNode != null && entitiesNode.ChildNodes != null)
            {
                foreach (XmlNode entityNode in entitiesNode.ChildNodes)
                {
                    Type type = Type.GetType("CADPadDB.CADEntity." + entityNode.Name);
                    if (type == null)
                    {
                        continue;
                    }
                    Entity ent = Activator.CreateInstance(type) as Entity;
                    if (ent == null)
                    {
                        continue;
                    }
                    filerImpl.curXmlNode = entityNode;
                    ent.XmlIn(filerImpl);
                    this._AppendEntity(ent);
                }
            }
            filerImpl.curXmlNode = curXmlNode;
        }

        #region IDBObjectContainer
        public IEnumerator<Entity> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
