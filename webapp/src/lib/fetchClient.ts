'use server';

import { auth } from "@/auth";
import { notFound } from "next/navigation";



export async function fetchClient<T>(
    url: string, method: 'GET' | 'POST' | 'PUT' | 'DELETE',
    options: Omit<RequestInit,'body'> & {body?:unknown} = {}) : Promise<{data : T | null,error? : {message: string, status: number}}> {

        const {body,...rest}= options;
        const apiUrl = process.env.API_URL;

        if(!apiUrl)
            throw new Error('API_URL is not set');

        const session = await auth();

        const headers:HeadersInit = {
            'Content-Type': 'application/json',
            ...(rest.headers || {}),
            // Access token is added to the headers for authentication.
            ...(session?.accessToken ? {Authorization: `Bearer ${session.accessToken}`} : {})
        };

        const response = await fetch(`${apiUrl}${url}`,{
            method,
            headers,
            ...(body ? {body : JSON.stringify(body)} : {}),
            ...rest
        })

        const contentType = response.headers.get('content-type');
        const isJson = contentType?.includes('application/json') || contentType?.includes('application/problem+json');
        const parsed = isJson ? await response.json() : await response.text();

        if(!response.ok){
            if(response.status === 404)
            {
                return notFound();
            }
            if(response.status === 500)
            {
                throw new Error('Server Error. Please try again later');
            }

            let message = '';

            if(response.status === 401){
                const authHeader  = response.headers.get('WWW-Authenticate');
                if(authHeader?.includes('error_description')){
                    const match = authHeader.match(/error_description="(.+?)"/);
                    if(match){
                        message=match[1];
                    }else{
                        message = 'You must be logged in to do that.';
                    }
                }
            }

            if(!message){
                if(typeof parsed === 'string'){
                    message = parsed;
                }
                else if (parsed?.message){
                    message = parsed.message;
                }
                else{
                    message = getFallbackMessage(response.status);
                }
            }

            return {data:null,error:{message,status:response.status}};

            // throw new Error(`${errorData.message || 'An error occured'}`);
        }

        return {data : parsed as T};
   
}


function getFallbackMessage(status: number): string {
    switch(status){
        case 400:
            return 'Bad Request. Please check your request and try again.';
        case 403:
            return 'Forbidden. You are not authorized to access this resource.';
        case 404:
            return 'Not Found. The resource you are looking for does not exist.';
        case 500:
            return 'Internal Server Error. Please try again later';
        default:
            return 'An error occured. Please try again later';
    }
}