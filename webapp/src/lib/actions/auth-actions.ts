'use server';

import { auth } from "@/auth";
import { fetchClient } from "../fetchClient";

export async function TestAuth(){
    return fetchClient<string>(`/test/auth`, 'GET');
}

export async function getCurrentUser(){
    try{
        const session = await auth();

        if(!session) return null;

        return session.user;
    
    }
    catch(error: unknown){
        console.log(error);
        return null;
    }
}

export async function getSession(){
    try{
        const session = await auth();
        return session;
    }
    catch(error: unknown){
        console.log(error);
        return null;
    }
}