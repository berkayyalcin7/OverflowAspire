'use server'

import { fetchClient } from "../fetchClient";
import { Question } from "../types";

export async function getQuestions(tag?: string) {
    let url = '/api/questions';

    if(tag) {
        url += `?tag=${tag}`;
    }

    // const response = await fetch(url);

    // if(!response.ok)
    // {
    //     throw new Error('Failed to fetch questions');
    // }

    // return response.json();

    return fetchClient<Question[]>(url, 'GET');
}

export async function getQuestionById(id: string) {
    
    const url = `/api/questions/${id}`;

    return fetchClient<Question>(url, 'GET');
}

export async function searchQuestions(query: string) {
    // Query parametresini encode ederek özel karakterlerde isteklerin bozulmamasını sağla
    const encodedQuery = encodeURIComponent(query);
    return fetchClient<Question[]>(`/search?query=${encodedQuery}`, 'GET');
}