'use client';
import { AcademicCapIcon, ArrowRightIcon, LightBulbIcon } from "@heroicons/react/24/solid";
import { Button } from "@heroui/react";
import Link from "next/link";

export default function Home() {
  return (
    <div className='flex flex-col min-h-[calc(100vh-160px)]'>
      {/* Hero Section */}
      <div className='flex flex-col items-center justify-center py-20 px-6 from-primary/5 to-transparent dark:from-primary/10 dark:to-transparent'>
        <div className='flex flex-col items-center justify-center gap-6 max-w-4xl text-center'>
          <div className='relative'>
            <AcademicCapIcon className='h-32 w-32 text-secondary dark:text-secondary-400 transition-colors' />
            <div className='absolute -top-2 -right-2 h-8 w-8 bg-success rounded-full flex items-center justify-center'>
              <LightBulbIcon className='h-5 w-5 text-white' />
            </div>
          </div>
          
          <div className='space-y-4'>
            <h1 className='text-5xl md:text-6xl font-bold text-foreground dark:text-foreground-100'>
              Welcome to <span className='text-secondary'>Overflow</span>
            </h1>
            <p className='text-xl md:text-2xl text-foreground-600 dark:text-foreground-400 max-w-2xl mx-auto'>
              A community-driven Q&A platform where developers share knowledge, solve problems, and grow together.
            </p>
          </div>

          <div className='flex flex-wrap gap-4 justify-center mt-4'>
            <Button 
              as={Link}
              href='/questions'
              color='secondary' 
              variant='solid'
              size='lg'
              endContent={<ArrowRightIcon className='h-5 w-5' />}
              className='font-semibold'
            >
              Browse Questions
            </Button>
    
          </div>
        </div>
      </div>

    </div>
  );
}
