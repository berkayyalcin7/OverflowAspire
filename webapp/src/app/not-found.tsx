'use client';
import { LinkComponent } from "@/components/nav/LinkComponents";
import { Button } from "@heroui/react";


export default function NotFound() {
    return (
        <div className="h-full flex items-center justify-center">
            <div className='text-center space-y-6'>
                <h1 className='text-4xl font-bold'>404 - Page Not Found</h1>
                <p className='text-lg text-foreground-500'>
                    The Page you are looking for does not exist.
                </p>
                <Button color='primary' variant='solid' as={LinkComponent} href='/'>Go back to home</Button>
            </div>
        </div>
    )
}