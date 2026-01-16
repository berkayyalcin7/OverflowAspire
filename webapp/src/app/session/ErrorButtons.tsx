'use client';
import { triggerError } from "@/lib/actions/error-actions";
import { Button } from "@heroui/button";
import { useTransition } from "react";

export default function ErrorButtons() {

    const [pending,startTransition]=useTransition();

    const onClick = (code :number)=>{
        startTransition(async ()=>{
            await triggerError(code);
        });
    }

  return (
    <div className='flex gap-6 items-center mt-6 w-full justify-center'>
        {[400,401,403,404,500].map(code=>(
            <Button key={code} 
            onPress={async ()=>onClick(code)} 
            color='primary' 
            variant='solid'>
               Test {code}</Button>
        ))}
    </div>
  )
}
