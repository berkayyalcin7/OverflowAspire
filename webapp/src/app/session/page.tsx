'use client';
import AuthTestButton from "./AuthTestButton";
import ErrorButtons from "./ErrorButtons";


export default function Page(){
    return (
        <div className='flex items-center gap-3 justify-center mt-6'>
            <ErrorButtons />
            <AuthTestButton/>
        </div>
    )
}