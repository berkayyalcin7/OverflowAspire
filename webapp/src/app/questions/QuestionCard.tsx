'use client';

import { Question } from "@/lib/types";
import { CheckIcon } from "@heroicons/react/24/solid";
import { Avatar, Chip, Link } from "@heroui/react";
import {clsx} from "clsx";

type Props = {
    question:Question;
}


export default function QuestionCard({question}:Props) {
    return (
        <div className="flex gap-6 px-6">
            <div className='flex flex-col items-end text-sm gap-3 min-w-24 text-neutral-700 dark:text-neutral-300'>
                <div>
                    {question.votes} {question.votes === 1 ? 'vote' : 'votes'}
                </div>
                <div className={clsx('flex justify-end rounded',{
                    'border-2 border-success':question.answerCount > 0,
                    'bg-success-600 text-default-50':question.hasAcceptedAnswer,
                })}>
                    <span className={clsx('flex items-center gap-2',{
                        'p-1':question.answerCount>0
                    })}>

                    {question.hasAcceptedAnswer && (
                        <CheckIcon className='h-4 w-4' strokeWidth={4}/>
                    )}

                    {question.answerCount} {question.answerCount === 1 ? 'answer' : 'answers'}

                    </span>
                </div>
                <div>
                    {question.viewCount} {question.viewCount === 1 ? 'view' : 'views'}
                </div>
            </div>

            <div className="flex flex-1 flex-col gap-2 min-h-32">
                <Link href={`/questions/${question.id}`}
                    className='text-primary font-semibold hover:underline first-letter:uppercase text-lg'
                >
                    {question.title}
                </Link>

                <div className='line-clamp-2 text-sm text-neutral-600 dark:text-neutral-300' dangerouslySetInnerHTML={{__html: question.content}}/>

                <div className='flex justify-between items-center pt-2 mt-auto'>
                    <div className='flex gap-2'>
                        {question.tagSlugs.map(slug=>(
                            <Chip 
                            key={slug} 
                            variant='bordered' as={Link} 
                            href={`/questions?tag=${slug}`} 
                            className='text-xs'
                            >
                                {slug}
                            </Chip>
                        ))}
                    </div>

                    <div className='text-sm flex items-center gap-2'>
                        <Avatar
                        className='h-6 w-6'
                        color="secondary"
                        name={question.askerDisplayName.charAt(0).toUpperCase()}
                        />
                        <Link href={`/profiles/${question.askerId}`} className='text-secondary hover:underline'>
                            {question.askerDisplayName}
                        </Link>
                        <span className='text-neutral-500 dark:text-neutral-400'>asked {question.createdAt}</span>
                    </div>
                </div>
            </div>
          
        </div>
    )
}