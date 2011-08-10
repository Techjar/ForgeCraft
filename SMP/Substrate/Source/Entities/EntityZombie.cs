﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityZombie : EntityMob
    {
        public static readonly SchemaNodeCompound ZombieSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Zombie"),
        });

        public EntityZombie ()
            : base("Zombie")
        {
        }

        public EntityZombie (EntityTyped e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, ZombieSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override EntityTyped Copy ()
        {
            return new EntityZombie(this);
        }

        #endregion
    }
}
