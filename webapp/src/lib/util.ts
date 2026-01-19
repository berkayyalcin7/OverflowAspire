import { addToast } from '@heroui/react';

export  function errorToast(error: {message: string, status?: number}) {
    return addToast({
        color:'danger',
        title:error.status,
        description:error.message || 'Something went wrong'
    });
}

export  function successToast(message: string, title?: string) 
{
    return addToast({
        color:'success',
        title:title || 'Success',
        description:message
    });
}

export function handleError(error: {message: string, status?: number}) {
    if(error.status === 500){
        throw error;
    }
    else{
        errorToast(error);
    }
}

