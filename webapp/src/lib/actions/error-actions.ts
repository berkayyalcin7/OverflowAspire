'use server';
import { fetchClient } from "../fetchClient";

export async function triggerError(code: number){
    return await fetchClient(`/api/questions/errors?code=${code}`, 'GET');
}