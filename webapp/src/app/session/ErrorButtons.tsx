'use client';
import { triggerError } from "@/lib/actions/error-actions";
import { handleError } from "@/lib/util";
import { Button } from "@heroui/button";
import { addToast } from "@heroui/toast";
import { useState, useTransition } from "react";

export default function ErrorButtons() {

    const [pending,startTransition]=useTransition();
    const [target,setTarget] = useState(0);

    const onClick = (code :number)=>{
      setTarget(code);
        startTransition(async ()=>{
           const {error} = await triggerError(code);
           if(error){
             handleError(error);
           }
           setTarget(0);
        });
    }

  return (
    <div className='flex gap-6 items-center mt-6 w-full justify-center'>
        {[400,401,403,404,500].map(code=>(
            <Button key={code} 
            onPress={async ()=>onClick(code)} 
            color='primary' 
            variant='solid'
            isLoading={pending && target === code}
            >
               Test {code}</Button>
        ))}
    </div>
  )
}
