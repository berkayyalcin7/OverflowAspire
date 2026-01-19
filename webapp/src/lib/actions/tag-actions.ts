'use server';

import { fetchClient } from "../fetchClient";
import { Tag } from "../types";

export async function getTags() {
    return await fetchClient<Tag[]>('/api/tags', 'GET', {cache: 'force-cache' , next: {revalidate: 360}});
}