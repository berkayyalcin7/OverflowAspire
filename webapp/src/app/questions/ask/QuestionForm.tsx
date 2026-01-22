'use client';

import { useTagStore } from "@/lib/useTagStore";
import { Form, Input, Textarea } from "@heroui/react";

export default function QuestionForm() {

    const tags = useTagStore(state => state.tags);

  return (
    <Form className='flex flex-col gap-3 p-6 shadow-xl bg-white dark:bg-black'>
        <div className='flex flex-col gap-3 w-full'>
            <h3 className='text-2xl font-semibold'> Title </h3>
            <Input 
            type='text' 
            placeholder='e.g how would you truncate text in talwind' 
            className='w-full'
            label='Be specific and imagine you are asking a question to another developer.'       
            required />
        </div>
        <div className='flex flex-col gap-3 w-full'>
            <h3 className='text-2xl font-semibold'> Body </h3>
            <Textarea 
            type='text' 
            labelPlacement='outside-top' 
            className='w-full'
            label='Include all the information someone would need to answer your question.'    
            minRows={10}>
                
            </Textarea>
        </div>
    </Form>
  )
}
