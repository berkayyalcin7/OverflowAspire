'use client'
import { getSession } from "@/lib/actions/auth-actions";
import AuthTestButton from "./AuthTestButton";
import ErrorButtons from "./ErrorButtons";
import { Snippet } from "@heroui/react";
import { useEffect, useState } from "react";


export default function Page(){
    const [session, setSession] = useState<unknown>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const loadSession = async () => {
            try {
                const sessionData = await getSession();
                setSession(sessionData);
            } catch (error) {
                console.error('Failed to load session:', error);
                setSession(null);
            } finally {
                setLoading(false);
            }
        };
        
        void loadSession();
    }, []);

    return (

        <div className='px-6'>

            <div className='text-center'>
                <h3 className='text-3xl'>Session Dashboard</h3>
            </div>

            {loading ? (
                <div className='text-center mt-4'>Loading session...</div>
            ) : (
                <Snippet symbol='' color="primary" classNames={{
                    base:'w-full mt-4',
                    pre:'text-wrap whitespace-pre-wrap break-all'
                }}>
                    {JSON.stringify(session,null,2)}
                </Snippet>
            )}

            <div className='flex items-center gap-3 justify-center mt-6'>
                <ErrorButtons />
                <AuthTestButton/>
            </div>
        </div>


      
    )
}