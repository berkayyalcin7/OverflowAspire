'use server';

import { fetchClient } from "../fetchClient";

export async function TestAuth(){
    return fetchClient<string>(`/test/auth`, 'GET');
}