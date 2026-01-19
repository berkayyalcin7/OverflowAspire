import { create } from "zustand";
import { Tag } from "./types"

type TagStore = {
    tags:Tag[];
    setTags:(tags:Tag[])=>void;
    getTagBySlug:(slug:string)=>Tag|undefined;
}

export const useTagStore = create<TagStore>((set,get)=>(
    {
        tags:[],
        setTags:(tags:Tag[])=>set({tags}),
        getTagBySlug:(slug:string)=>get().tags.find(tag=>tag.slug === slug),
    }
))